using static TaskList.Domain.Entity.TaskItem;
using TaskList.Domain.Entity;

namespace TaskList.Domain
{

    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetByListIdAsync(int listId, CancellationToken cancellationToken = default);
        Task<TaskItem> GetByIdAsync(int listId, int taskId, CancellationToken cancellationToken = default);
        Task<TaskItem> AddAsync(TaskItem entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(TaskItem entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(TaskItem entity, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskItem>> SearchAsync(int? listId, string searchTerm, Priority? priority, bool? isCompleted, CancellationToken cancellationToken = default);
    }
}
