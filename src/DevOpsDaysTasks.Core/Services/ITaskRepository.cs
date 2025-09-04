using DevOpsDaysTasks.Core.Models;

namespace DevOpsDaysTasks.Core.Services;

public interface ITaskRepository
{
    Task EnsureCreatedAsync(CancellationToken ct = default);
    Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken ct = default);
    Task<TaskItem> AddAsync(TaskItem item, CancellationToken ct = default);
    Task UpdateAsync(TaskItem item, CancellationToken ct = default);
    Task RemoveAsync(long id, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<TaskItem> items, CancellationToken ct = default);
}
