using TaskList.Domain.DTO;

namespace TaskList.Domain.RepositoryInerfaces
{
    public interface ITaskListRepository
    {
        Task<IEnumerable<TaskList.Domain.Entity.TaskList>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TaskList.Domain.Entity.TaskList> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TaskList.Domain.Entity.TaskList> AddAsync(TaskList.Domain.Entity.TaskList entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(TaskList.Domain.Entity.TaskList entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(TaskList.Domain.Entity.TaskList entity, CancellationToken cancellationToken = default);
    }
}
