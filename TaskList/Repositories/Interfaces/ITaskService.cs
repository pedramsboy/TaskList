using static TaskList.Models.Domain.TaskItem;
using TaskList.Models.DTO;

namespace TaskList.Repositories.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItemDto>> GetTasksByListIdAsync(int listId, string sortBy, bool isAscending);
        Task<TaskItemDto> GetTaskByIdAsync(int listId, int taskId);
        Task<TaskItemDto> CreateTaskAsync(int listId, CreateTaskItemDto createDto);
        Task UpdateTaskAsync(int listId, int taskId, UpdateTaskItemDto updateDto);
        Task DeleteTaskAsync(int listId, int taskId);
        Task MarkTaskAsDoneAsync(int listId, int taskId);
        Task<IEnumerable<TaskItemDto>> SearchTasksAsync(int? listId, string searchTerm, Priority? priority, bool? isCompleted);
    }
}
