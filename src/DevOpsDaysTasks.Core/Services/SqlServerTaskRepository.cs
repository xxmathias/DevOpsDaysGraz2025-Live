// Core/Services/SqlServerTaskRepository.cs
using DevOpsDaysTasks.Core.Models;
using Microsoft.Data.SqlClient;

namespace DevOpsDaysTasks.Core.Services;

public class SqlServerTaskRepository : ITaskRepository
{
    private readonly string _connectionString;

    public SqlServerTaskRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task EnsureCreatedAsync(CancellationToken ct = default)
    {
        // Ensure DB exists + schema (simple inline DDL for the workshop)
        var builder = new SqlConnectionStringBuilder(_connectionString);
        var database = builder.InitialCatalog;
        builder.InitialCatalog = "master";

        await using (var con = new SqlConnection(builder.ConnectionString))
        {
            await con.OpenAsync(ct);
            await using var cmd = con.CreateCommand();
            cmd.CommandText = $@"
IF DB_ID(@db) IS NULL CREATE DATABASE [{database}];
";
            cmd.Parameters.AddWithValue("@db", database);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        await using (var con = new SqlConnection(_connectionString))
        {
            await con.OpenAsync(ct);
            await using var cmd = con.CreateCommand();
            cmd.CommandText = @"
IF OBJECT_ID('dbo.Tasks') IS NULL
BEGIN
  CREATE TABLE dbo.Tasks(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(100) NOT NULL,
    IsDone BIT NOT NULL DEFAULT(0)
  );
END
";
            await cmd.ExecuteNonQueryAsync(ct);
        }
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken ct = default)
    {
        var list = new List<TaskItem>();
        await using var con = new SqlConnection(_connectionString);
        await con.OpenAsync(ct);
        await using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT Id, Title, IsDone FROM dbo.Tasks ORDER BY IsDone ASC, Id ASC;";
        await using var r = await cmd.ExecuteReaderAsync(ct);
        while (await r.ReadAsync(ct))
        {
            list.Add(new TaskItem
            {
                Id = (int)r.GetInt64(0),
                Title = r.GetString(1),
                IsDone = r.GetBoolean(2),
            });
        }
        return list;
    }

    public async Task<TaskItem> AddAsync(TaskItem item, CancellationToken ct = default)
    {
        await using var con = new SqlConnection(_connectionString);
        await con.OpenAsync(ct);
        await using var cmd = con.CreateCommand();
        cmd.CommandText = @"INSERT INTO dbo.Tasks(Title, IsDone) 
                            OUTPUT INSERTED.Id VALUES (@t,@d)";
        cmd.Parameters.AddWithValue("@t", item.Title);
        cmd.Parameters.AddWithValue("@d", item.IsDone);
        item.Id = (int)(long)(await cmd.ExecuteScalarAsync(ct) ?? 0m);
        return item;
    }

    public async Task AddRangeAsync(IEnumerable<TaskItem> items, CancellationToken ct = default)
    {
        await using var con = new SqlConnection(_connectionString);
        await con.OpenAsync(ct);
        await using var tx = await con.BeginTransactionAsync(ct);
        foreach (var it in items)
        {
            var cmd = con.CreateCommand();
            cmd.Transaction = (SqlTransaction)tx;
            cmd.CommandText = "INSERT INTO dbo.Tasks(Title, IsDone) VALUES (@t,@d)";
            cmd.Parameters.AddWithValue("@t", it.Title);
            cmd.Parameters.AddWithValue("@d", it.IsDone);
            await cmd.ExecuteNonQueryAsync(ct);
        }
        await tx.CommitAsync(ct);
    }

    public async Task UpdateAsync(TaskItem item, CancellationToken ct = default)
    {
        await using var con = new SqlConnection(_connectionString);
        await con.OpenAsync(ct);
        await using var cmd = con.CreateCommand();
        cmd.CommandText = "UPDATE dbo.Tasks SET Title=@t, IsDone=@d WHERE Id=@id";
        cmd.Parameters.AddWithValue("@t", item.Title);
        cmd.Parameters.AddWithValue("@d", item.IsDone);
        cmd.Parameters.AddWithValue("@id", item.Id);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task RemoveAsync(long id, CancellationToken ct = default)
    {
        await using var con = new SqlConnection(_connectionString);
        await con.OpenAsync(ct);
        await using var cmd = con.CreateCommand();
        cmd.CommandText = "DELETE FROM dbo.Tasks WHERE Id=@id";
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
