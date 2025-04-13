using static TaskList.Domain.Entity.TaskItem;
using TaskList.Domain.DTO;

namespace TaskList.Application.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItemDto>> GetTasksByListIdAsync(int listId, string sortBy, bool isAscending, CancellationToken cancellationToken = default);
        Task<TaskItemDto> GetTaskByIdAsync(int listId, int taskId, CancellationToken cancellationToken = default);
        Task<TaskItemDto> CreateTaskAsync(int listId, CreateTaskItemDto createDto, CancellationToken cancellationToken = default);
        Task UpdateTaskAsync(int listId, int taskId, UpdateTaskItemDto updateDto, CancellationToken cancellationToken = default);
        Task DeleteTaskAsync(int listId, int taskId, CancellationToken cancellationToken = default);
        Task MarkTaskAsDoneAsync(int listId, int taskId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskItemDto>> SearchTasksAsync(int? listId, string searchTerm, Priority? priority, bool? isCompleted, CancellationToken cancellationToken = default);
    }
}
