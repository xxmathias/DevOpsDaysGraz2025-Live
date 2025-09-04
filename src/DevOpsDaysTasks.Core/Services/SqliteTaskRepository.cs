using DevOpsDaysTasks.Core.Models;
using Microsoft.Data.Sqlite;

namespace DevOpsDaysTasks.Core.Services;

public class SqliteTaskRepository : ITaskRepository
{
    private readonly string dbFilePath;

    public SqliteTaskRepository(string filePath)
    {
        this.dbFilePath = filePath;
    }

    private SqliteConnection Create()
        => new($"Data Source={dbFilePath};Pooling=False");

    public async Task EnsureCreatedAsync(CancellationToken ct = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(dbFilePath)!);
        await using var con = Create();
        await con.OpenAsync(ct);
        var cmd = con.CreateCommand();
        cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Tasks (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  Title TEXT NOT NULL,
  IsDone INTEGER NOT NULL DEFAULT 0
);";
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken ct = default)
    {
        var list = new List<TaskItem>();
        await using var con = Create();
        await con.OpenAsync(ct);
        var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT Id, Title, IsDone FROM Tasks ORDER BY IsDone ASC, Id ASC;";
        await using var r = await cmd.ExecuteReaderAsync(ct);
        while (await r.ReadAsync(ct))
        {
            list.Add(new TaskItem
            {
                Id = r.GetInt32(0),
                Title = r.GetString(1),
                IsDone = r.GetInt64(2) == 1
            });
        }
        return list;
    }

    public async Task<TaskItem> AddAsync(TaskItem item, CancellationToken ct = default)
    {
        await using var con = Create();
        await con.OpenAsync(ct);
        var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO Tasks (Title, IsDone) VALUES ($t, $d); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$t", item.Title);
        cmd.Parameters.AddWithValue("$d", item.IsDone ? 1 : 0);
        var id = (int)(long)(await cmd.ExecuteScalarAsync(ct) ?? 0L);
        item.Id = id;
        return item;
    }

    public async Task AddRangeAsync(IEnumerable<TaskItem> items, CancellationToken ct = default)
    {
        await using var con = Create();
        await con.OpenAsync(ct);
        await using var tx = await con.BeginTransactionAsync(ct);
        foreach (var it in items)
        {
            var cmd = con.CreateCommand();
            cmd.CommandText = "INSERT INTO Tasks (Title, IsDone) VALUES ($t, $d);";
            cmd.Parameters.AddWithValue("$t", it.Title);
            cmd.Parameters.AddWithValue("$d", it.IsDone ? 1 : 0);
            await cmd.ExecuteNonQueryAsync(ct);
        }
        await tx.CommitAsync(ct);
    }

    public async Task UpdateAsync(TaskItem item, CancellationToken ct = default)
    {
        await using var con = Create();
        await con.OpenAsync(ct);
        var cmd = con.CreateCommand();
        cmd.CommandText = "UPDATE Tasks SET Title=$t, IsDone=$d WHERE Id=$id;";
        cmd.Parameters.AddWithValue("$t", item.Title);
        cmd.Parameters.AddWithValue("$d", item.IsDone ? 1 : 0);
        cmd.Parameters.AddWithValue("$id", item.Id);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task RemoveAsync(long id, CancellationToken ct = default)
    {
        await using var con = Create();
        await con.OpenAsync(ct);
        var cmd = con.CreateCommand();
        cmd.CommandText = "DELETE FROM Tasks WHERE Id=$id;";
        cmd.Parameters.AddWithValue("$id", id);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
