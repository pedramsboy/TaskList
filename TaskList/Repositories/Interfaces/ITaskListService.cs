using TaskList.Models.DTO;

namespace TaskList.Repositories.Interfaces
{
    public interface ITaskListService
    {
        Task<IEnumerable<TaskListDto>> GetAllTaskListsAsync();
        Task<TaskListDto> GetTaskListByIdAsync(int id);
        Task<TaskListDto> CreateTaskListAsync(CreateTaskListDto createDto);
        Task UpdateTaskListAsync(int id, UpdateTaskListDto updateDto);
        Task DeleteTaskListAsync(int id);
    }
}
