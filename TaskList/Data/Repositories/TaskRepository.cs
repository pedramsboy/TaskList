using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using TaskList.Data;
using TaskList.Domain;
using TaskList.Domain.DTO;
using TaskList.Domain.Entity;
using TaskList.Domain.RepositoryInerfaces;
using static TaskList.Domain.Entity.TaskItem;

namespace TaskList.Data.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskDbContext _context;

        public TaskRepository(TaskDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskItem>> GetByListIdAsync(int listId, CancellationToken cancellationToken = default)
        {
            return await _context.Tasks
                .AsNoTracking()
                .Where(t => t.TaskListId == listId)
                .ToListAsync(cancellationToken);
        }

        public async Task<TaskItem> GetByIdAsync(int listId, int taskId, CancellationToken cancellationToken = default)
        {
            return await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == taskId && t.TaskListId == listId, cancellationToken);
        }

        public async Task<TaskItem> AddAsync(TaskItem entity, CancellationToken cancellationToken = default)
        {
            _context.Tasks.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task UpdateAsync(TaskItem entity, CancellationToken cancellationToken = default)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.Tasks.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(TaskItem entity, CancellationToken cancellationToken = default)
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(entity, cancellationToken);
        }

        public async Task<IEnumerable<TaskItem>> SearchAsync(int? listId, string searchTerm, Priority? priorities, bool? isCompleted, CancellationToken cancellationToken = default)
        {
            var query = _context.Tasks
                .AsNoTracking()
                .AsQueryable();

            if (listId.HasValue)
            {
                query = query.Where(t => t.TaskListId == listId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(t =>
                    t.Title.Contains(searchTerm) ||
                    (t.Description != null && t.Description.Contains(searchTerm)));
            }

            if (priorities.HasValue)
            {
                query = query.Where(t => t.Priorities == priorities.Value);
            }

            if (isCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == isCompleted.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }
    }
}

