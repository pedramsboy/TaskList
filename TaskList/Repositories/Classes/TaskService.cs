using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using TaskList.Data;
using TaskList.Models.Domain;
using TaskList.Models.DTO;
using TaskList.Models.Enum;
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

        public async Task<IEnumerable<TaskItemDto>> GetAllByListIdAsync(int listId, TaskSortEnum sortBy, bool isAscending, CancellationToken cancellationToken = default)
        {
            // Check if task list exists (still needs tracking for existence check)
            var taskList = await _context.TaskLists
                .AsNoTracking()
                .FirstOrDefaultAsync(tl => tl.Id == listId, cancellationToken);

            if (taskList == null)
                throw new KeyNotFoundException("Task list not found");

            var query = _context.Tasks
                .AsNoTracking()
                .Where(t => t.TaskListId == listId)
                .AsQueryable();

            // Sorting
            query = sortBy switch
            {
                TaskSortEnum.Title => isAscending ? query.OrderBy(t => t.Title) : query.OrderByDescending(t => t.Title),
                TaskSortEnum.DueDate => isAscending ? query.OrderBy(t => t.DueDate) : query.OrderByDescending(t => t.DueDate),
                TaskSortEnum.Priorities => isAscending ? query.OrderBy(t => t.Priorities) : query.OrderByDescending(t => t.Priorities),
                TaskSortEnum.CreatedAt => isAscending ? query.OrderBy(t => t.CreatedAt) : query.OrderByDescending(t => t.CreatedAt),
                _ => isAscending ? query.OrderBy(t => t.Id) : query.OrderByDescending(t => t.Id)
            };

            var tasks = await query.ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<TaskItemDto>>(tasks);
        }

        public async Task<TaskItemDto> GetByIdAsync(int listId, int taskId, CancellationToken cancellationToken = default)
        {
            var task = await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == taskId && t.TaskListId == listId, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException("Task not found");

            return _mapper.Map<TaskItemDto>(task);
        }

        public async Task<TaskItemDto> CreateAsync(int listId, CreateTaskItemDto createDto, CancellationToken cancellationToken = default)
        {
            var taskList = await _context.TaskLists
                .FirstOrDefaultAsync(tl => tl.Id == listId, cancellationToken);

            if (taskList == null)
                throw new KeyNotFoundException("Task list not found");

            var task = _mapper.Map<TaskItem>(createDto);
            task.TaskListId = listId;

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<TaskItemDto>(task);
        }

        public async Task UpdateAsync(int listId, int taskId, UpdateTaskItemDto updateDto, CancellationToken cancellationToken = default)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && t.TaskListId == listId, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException("Task not found");

            if (task.IsDone)
                throw new InvalidOperationException("Completed tasks cannot be modified");

            _mapper.Map(updateDto, task);
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int listId, int taskId, CancellationToken cancellationToken = default)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && t.TaskListId == listId, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException("Task not found");

            task.IsDeleted = true;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkAsDoneAsync(int listId, int taskId, CancellationToken cancellationToken = default)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && t.TaskListId == listId, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException("Task not found");

            task.IsDone = true;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<TaskItemDto>> SearchAsync(int? listId, string searchTerm, Priority? priority, bool? isDone, CancellationToken cancellationToken = default)
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

            if (priority.HasValue)
            {
                query = query.Where(t => t.Priorities == priority.Value);
            }

            if (isDone.HasValue)
            {
                query = query.Where(t => t.IsDone == isDone.Value);
            }

            var tasks = await query.ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<TaskItemDto>>(tasks);
        }
    }
}
