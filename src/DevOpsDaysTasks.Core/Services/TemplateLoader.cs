using DevOpsDaysTasks.Core.Models;
using System.Xml.Linq;

namespace DevOpsDaysTasks.Core.Services;

public static class TemplateLoader
{
    public static IReadOnlyList<TaskItem> LoadDefaultTasks()
    {
        var prod = Path.Combine(AppPaths.TemplatesDir, "default-tasks.xml");
        var dev = FindUpwards(Path.Combine("data", "templates", "default-tasks.xml"), AppContext.BaseDirectory);
        var path = File.Exists(prod) ? prod : File.Exists(dev) ? dev : null;

        if (path is null)
            throw new FileNotFoundException("Template file not found at ./Templates/default-tasks.xml or ../../data/templates/default-tasks.xml");

        var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        return LoadDefaultTasks(stream);
    }

    public static IReadOnlyList<TaskItem> LoadDefaultTasks(Stream stream)
    {
        var xdoc = XDocument.Load(stream);
        var tasks = new List<TaskItem>();
        foreach (var t in xdoc.Root?.Elements("Task") ?? Enumerable.Empty<XElement>())
        {
            var title = (string?)t.Attribute("Title") ?? t.Element("Title")?.Value;
            var doneAttr = (string?)t.Attribute("Done");
            if (!string.IsNullOrWhiteSpace(title))
            {
                tasks.Add(new TaskItem
                {
                    Title = title.Trim(),
                    IsDone = doneAttr is not null &&
                             (doneAttr.Equals("true", StringComparison.OrdinalIgnoreCase) || doneAttr == "1")
                });
            }
        }
        return tasks;
    }

    private static string? FindUpwards(string relative, string start)
    {
        var dir = new DirectoryInfo(start);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, relative);
            if (File.Exists(candidate)) return candidate;
            dir = dir.Parent;
        }
        return null;
    }
}
