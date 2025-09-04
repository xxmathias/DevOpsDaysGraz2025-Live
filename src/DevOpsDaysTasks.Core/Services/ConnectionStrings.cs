namespace DevOpsDaysTasks.Core.Services;

public static class ConnectionStrings
{
    public static string SqlServer =>
        Environment.GetEnvironmentVariable("SQL_CONNECTION")
            ?? "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=DevOpsDaysTasks;Integrated Security=True;MultipleActiveResultSets=True";

    public static string Sqlite =>
        AppPaths.DbFilePath;
}
