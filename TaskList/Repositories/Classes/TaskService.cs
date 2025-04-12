using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using TaskList.Data;
using TaskList.Models.Domain;
using TaskList.Models.DTO;
using TaskList.Repositories.Interfaces;
using static TaskList.Models.Domain.TaskItem;

namespace TaskList.Repositories.Classes
{
    public class TaskService : ITaskService
    {
        private readonly TaskDbContext _context;
        private readonly IMapper _mapper;

        public TaskService(TaskDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskItemDto>> GetTasksByListIdAsync(int listId, string sortBy, bool isAscending, CancellationToken cancellationToken = default)
        {
            // Check if task list exists (still needs tracking for existence check)
            var taskList = await _context.TaskLists
                .AsNoTracking()
                .FirstOrDefaultAsync(tl => tl.Id == listId && !tl.IsDeleted, cancellationToken);

            if (taskList == null)
                throw new KeyNotFoundException("Task list not found");

            var query = _context.Tasks
                .AsNoTracking()
                .Where(t => t.TaskListId == listId && !t.IsDeleted)
                .AsQueryable();

            // Sorting
            query = sortBy?.ToLower() switch
            {
                "title" => isAscending ? query.OrderBy(t => t.Title) : query.OrderByDescending(t => t.Title),
                "duedate" => isAscending ? query.OrderBy(t => t.DueDate) : query.OrderByDescending(t => t.DueDate),
                "priority" => isAscending ? query.OrderBy(t => t.Priorities) : query.OrderByDescending(t => t.Priorities),
                "createdat" => isAscending ? query.OrderBy(t => t.CreatedAt) : query.OrderByDescending(t => t.CreatedAt),
                _ => isAscending ? query.OrderBy(t => t.Id) : query.OrderByDescending(t => t.Id)
            };

            var tasks = await query.ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<TaskItemDto>>(tasks);
        }

        public async Task<TaskItemDto> GetTaskByIdAsync(int listId, int taskId, CancellationToken cancellationToken = default)
        {
            var task = await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == taskId && t.TaskListId == listId && !t.IsDeleted, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException("Task not found");

            return _mapper.Map<TaskItemDto>(task);
        }

        public async Task<TaskItemDto> CreateTaskAsync(int listId, CreateTaskItemDto createDto, CancellationToken cancellationToken = default)
        {
            var taskList = await _context.TaskLists
                .FirstOrDefaultAsync(tl => tl.Id == listId && !tl.IsDeleted, cancellationToken);

            if (taskList == null)
                throw new KeyNotFoundException("Task list not found");

            var task = _mapper.Map<TaskItem>(createDto);
            task.TaskListId = listId;

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<TaskItemDto>(task);
        }

        public async Task UpdateTaskAsync(int listId, int taskId, UpdateTaskItemDto updateDto, CancellationToken cancellationToken = default)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && t.TaskListId == listId && !t.IsDeleted, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException("Task not found");

            if (task.IsCompleted)
                throw new InvalidOperationException("Completed tasks cannot be modified");

            _mapper.Map(updateDto, task);
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteTaskAsync(int listId, int taskId, CancellationToken cancellationToken = default)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && t.TaskListId == listId && !t.IsDeleted, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException("Task not found");

            task.IsDeleted = true;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkTaskAsDoneAsync(int listId, int taskId, CancellationToken cancellationToken = default)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && t.TaskListId == listId && !t.IsDeleted, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException("Task not found");

            task.IsCompleted = true;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<TaskItemDto>> SearchTasksAsync(int? listId, string searchTerm, Priority? priority, bool? isCompleted, CancellationToken cancellationToken = default)
        {
            var query = _context.Tasks
                .AsNoTracking()
                .Where(t => !t.IsDeleted)
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

            if (priority.HasValue)
            {
                query = query.Where(t => t.Priorities == priority.Value);
            }

            if (isCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == isCompleted.Value);
            }

            var tasks = await query.ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<TaskItemDto>>(tasks);
        }
    }
}
