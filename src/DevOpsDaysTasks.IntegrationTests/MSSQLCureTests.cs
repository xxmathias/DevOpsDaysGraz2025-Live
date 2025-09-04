using DevOpsDaysTasks.Core.Models;
using DevOpsDaysTasks.Core.Services;
using System.Text;

namespace DevOpsDaysTasks.IntegrationTests;

public class MssqlCrudTests : IClassFixture<MssqlFixture>
{
    private readonly ITaskRepository _repo;

    public MssqlCrudTests(MssqlFixture fx)
    {
        _repo = fx.Repository;
    }

    [Fact]
    public async Task Crud_Works_On_Mssql()
    {
        var added = await _repo.AddAsync(new TaskItem { Title = "MSSQL item" });
        Assert.True(added.Id > 0);

        var all = await _repo.GetAllAsync();
        Assert.Contains(all, t => t.Title == "MSSQL item" && !t.IsDone);

        var item = all.First(t => t.Title == "MSSQL item");
        item.IsDone = true;
        await _repo.UpdateAsync(item);

        var afterUpdate = await _repo.GetAllAsync();
        Assert.True(afterUpdate.First(t => t.Id == item.Id).IsDone);

        await _repo.RemoveAsync(item.Id);
        var afterDelete = await _repo.GetAllAsync();
        Assert.DoesNotContain(afterDelete, t => t.Id == item.Id);
    }

    [Fact]
    public async Task Import_From_Templates_Populates_Mssql_Db()
    {
        var templatesDir = Path.Combine(AppContext.BaseDirectory, "Templates");
        Directory.CreateDirectory(templatesDir);
        var xml = """
<Tasks>
  <Task Title="Alpha" Done="false"/>
  <Task Title="Beta"  Done="true"/>
</Tasks>
""";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        var tasks = TemplateLoader.LoadDefaultTasks(stream);
        await _repo.AddRangeAsync(tasks);

        var all = await _repo.GetAllAsync();
        Assert.Equal(2, all.Count);
        Assert.Contains(all, t => t.Title == "Alpha" && t.IsDone == false);
        Assert.Contains(all, t => t.Title == "Beta" && t.IsDone == true);


    }
}
