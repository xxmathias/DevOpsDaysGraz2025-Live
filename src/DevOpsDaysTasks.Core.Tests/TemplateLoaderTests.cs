using DevOpsDaysTasks.Core.Services;
using System.Text;

namespace DevOpsDaysTasks.Core.Tests;

public class TemplateLoaderTests
{
    [Fact]
    public void LoadDefaultTasks_Parses_Title_And_Done()
    {
        // Arrange
        var xml = """
<Tasks>
  <Task Title=" Buy groceries " Done="true"/>
  <Task Title="Prepare slides" Done="0"/>
</Tasks>
""";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var tasks = TemplateLoader.LoadDefaultTasks(stream);

        // Assert
        Assert.Equal(2, tasks.Count);
        Assert.Equal("Buy groceries", tasks[0].Title);   // trimmed
        Assert.True(tasks[0].IsDone);                    // "true" -> true
        Assert.False(tasks[1].IsDone);                   // "0" -> false
    }

    [Fact]
    public void LoadDefaultTasks_Skips_Empty_Titles()
    {
        var xml = """
<Tasks>
  <Task Title="  " Done="true"/>
  <Task Done="true"><Title></Title></Task>
  <Task Title="Valid" Done="false"/>
</Tasks>
""";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var tasks = TemplateLoader.LoadDefaultTasks(stream);

        Assert.Single(tasks); // Tasks without Title or an empty Tilte are skipped
        Assert.Equal("Valid", tasks[0].Title);
    }
}
