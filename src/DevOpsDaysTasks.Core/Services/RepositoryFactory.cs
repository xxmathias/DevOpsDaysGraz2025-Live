namespace DevOpsDaysTasks.Core.Services;

public enum DbKind { Sqlite, SqlServer }

public static class RepositoryFactory
{
    public static ITaskRepository Create(DbKind dbKind, string? connectionString = null)
    {
        return dbKind switch
        {
            DbKind.SqlServer => new SqlServerTaskRepository(connectionString ?? ConnectionStrings.SqlServer),
            DbKind.Sqlite => new SqliteTaskRepository(connectionString ?? ConnectionStrings.Sqlite),
            _ => throw new NotImplementedException()
        };
    }
}
