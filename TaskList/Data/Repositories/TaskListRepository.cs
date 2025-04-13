using Microsoft.EntityFrameworkCore;
using TaskList.Data;
using TaskList.Domain.Entity;
using TaskList.Domain.RepositoryInerfaces;

namespace TaskList.Repositories.Classes
{
    public class TaskListRepository : ITaskListRepository
    {
        private readonly TaskDbContext _context;

        public TaskListRepository(TaskDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskList.Domain.Entity.TaskList>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.TaskLists
                .AsNoTracking()
                .Include(tl => tl.Tasks)
                .ToListAsync(cancellationToken);
        }

        public async Task<TaskList.Domain.Entity.TaskList> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.TaskLists
                .AsNoTracking()
                .Include(tl => tl.Tasks)
                .FirstOrDefaultAsync(tl => tl.Id == id, cancellationToken);
        }

        public async Task<TaskList.Domain.Entity.TaskList> AddAsync(TaskList.Domain.Entity.TaskList entity, CancellationToken cancellationToken = default)
        {
            _context.TaskLists.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task UpdateAsync(TaskList.Domain.Entity.TaskList entity, CancellationToken cancellationToken = default)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.TaskLists.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(TaskList.Domain.Entity.TaskList entity, CancellationToken cancellationToken = default)
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            foreach (var task in entity.Tasks)
            {
                task.IsDeleted = true;
                task.UpdatedAt = DateTime.UtcNow;
            }

            await UpdateAsync(entity, cancellationToken);
        }
    }
}