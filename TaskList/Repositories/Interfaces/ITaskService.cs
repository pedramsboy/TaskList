using static TaskList.Models.Domain.TaskItem;
using TaskList.Models.DTO;
using TaskList.Models.Enum;

namespace TaskList.Repositories.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItemDto>> GetAllByListIdAsync(int listId, TaskSortEnum sortBy, bool isAscending, CancellationToken cancellationToken = default);
        Task<TaskItemDto> GetByIdAsync(int listId, int taskId, CancellationToken cancellationToken = default);
        Task<TaskItemDto> CreateAsync(int listId, CreateTaskItemDto createDto, CancellationToken cancellationToken = default);
        Task UpdateAsync(int listId, int taskId, UpdateTaskItemDto updateDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int listId, int taskId, CancellationToken cancellationToken = default);
        Task MarkAsDoneAsync(int listId, int taskId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskItemDto>> SearchAsync(int? listId, string searchTerm, Priority? priority, bool? isCompleted, CancellationToken cancellationToken = default);
    }
}
