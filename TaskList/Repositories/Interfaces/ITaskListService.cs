using TaskList.Models.DTO;

namespace TaskList.Repositories.Interfaces
{
    public interface ITaskListService
    {
        Task<IEnumerable<TaskListDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TaskListDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TaskListDto> CreateAsync(CreateTaskListDto createDto, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, UpdateTaskListDto updateDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<string> UpdateImageAsync(int id, IFormFile imageFile, CancellationToken cancellationToken = default);
        Task RemoveImageAsync(int id, CancellationToken cancellationToken = default);
    }
}
