using DevOpsDaysTasks.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DevOpsDaysTasks.IntegrationTests;

public class MssqlFixture : IAsyncLifetime
{
    public ITaskRepository Repository { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var dbKind = DbKind.SqlServer;
        // If no local DB is installed we can fallback to Sqlite
        // var dbKind = DbKind.Sqlite;

        if (dbKind == DbKind.SqlServer)
        {
            var connectionString = Environment.GetEnvironmentVariable("MSSQL_CONNECTION")
               ?? "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=DevOpsDaysTasksIntegrationTests;Integrated Security=True;MultipleActiveResultSets=True";

            // Fail fast (skip) if the server isn’t reachable
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                builder.InitialCatalog = "master";

                using var con = new SqlConnection(builder.ConnectionString);
                await con.OpenAsync();
                if (con.State != ConnectionState.Open)
                    throw new Exception("Connection did not open.");
            }
            catch (Exception ex)
            {
                throw new Exception($"SQL Server is not reachable.\n{ex.Message}");
            }

            // Ensure the DB/schema exists via repo EnsureCreated
            Repository = RepositoryFactory.Create(DbKind.SqlServer, connectionString);
        }
        else
        {
            Repository = RepositoryFactory.Create(DbKind.Sqlite, "./DB/DevOpsDaysTasksIntegrationTests.db");
        }

        await Repository.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        // Clean DB
        var entries = await Repository.GetAllAsync();
        foreach (var entry in entries)
        {
            await Repository.RemoveAsync(entry.Id);
        }
    }
}
