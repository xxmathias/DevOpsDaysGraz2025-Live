namespace DevOpsDaysTasks.Core.Services;

public static class AppPaths
{
    public static string AppDataDir
    {
        get
        {
            var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var path = Path.Combine(baseDir, "DevOpsDaysTasks");
            Directory.CreateDirectory(path);
            return path;
        }
    }

    public static string DbFilePath
    {
        get
        {
            return Path.Combine(AppDataDir, "DevOpsDaysTasks.db");
        }
    }

    public static string TemplatesDir
    {
        get
        {
            // at runtime, next to the executable
            var baseDir = AppContext.BaseDirectory;
            return Path.Combine(baseDir, "Templates");
        }
    }

    public static string HelpFilePath
    {
        get
        {
            var baseDir = AppContext.BaseDirectory;
            return Path.Combine(baseDir, "Help", "Help.pdf");
        }
    }
}
