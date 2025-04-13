using AutoMapper;
using static TaskList.Domain.Entity.TaskItem;
using TaskList.Application.Interfaces;
using TaskList.Domain.DTO;
using TaskList.Domain.Entity;
using TaskList.Domain.RepositoryInerfaces;
using static TaskList.Domain.ITaskRepository;

namespace TaskList.Application.Classes
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITaskListRepository _taskListRepository;
        private readonly IMapper _mapper;

        public TaskService(
            ITaskRepository taskRepository,
            ITaskListRepository taskListRepository,
            IMapper mapper)
        {
            _taskRepository = taskRepository;
            _taskListRepository = taskListRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskItemDto>> GetTasksByListIdAsync(int listId, string sortBy, bool isAscending, CancellationToken cancellationToken = default)
        {
            // Verify task list exists
            var taskList = await _taskListRepository.GetByIdAsync(listId, cancellationToken);
            if (taskList == null)
                throw new KeyNotFoundException("Task list not found");

            var tasks = await _taskRepository.GetByListIdAsync(listId, cancellationToken);

            // Apply sorting in memory (could also be done in repository if needed)
            var sortedTasks = sortBy?.ToLower() switch
            {
                "title" => isAscending ? tasks.OrderBy(t => t.Title) : tasks.OrderByDescending(t => t.Title),
                "duedate" => isAscending ? tasks.OrderBy(t => t.DueDate) : tasks.OrderByDescending(t => t.DueDate),
                "priority" => isAscending ? tasks.OrderBy(t => t.Priorities) : tasks.OrderByDescending(t => t.Priorities),
                "createdat" => isAscending ? tasks.OrderBy(t => t.CreatedAt) : tasks.OrderByDescending(t => t.CreatedAt),
                _ => isAscending ? tasks.OrderBy(t => t.Id) : tasks.OrderByDescending(t => t.Id)
            };

            return _mapper.Map<IEnumerable<TaskItemDto>>(sortedTasks);
        }

        public async Task<TaskItemDto> GetTaskByIdAsync(int listId, int taskId, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetByIdAsync(listId, taskId, cancellationToken);
            if (task == null)
                throw new KeyNotFoundException("Task not found");

            return _mapper.Map<TaskItemDto>(task);
        }

        public async Task<TaskItemDto> CreateTaskAsync(int listId, CreateTaskItemDto createDto, CancellationToken cancellationToken = default)
        {
            // Verify task list exists
            var taskList = await _taskListRepository.GetByIdAsync(listId, cancellationToken);
            if (taskList == null)
                throw new KeyNotFoundException("Task list not found");

            var task = _mapper.Map<TaskItem>(createDto);
            task.TaskListId = listId;

            task = await _taskRepository.AddAsync(task, cancellationToken);
            return _mapper.Map<TaskItemDto>(task);
        }

        public async Task UpdateTaskAsync(int listId, int taskId, UpdateTaskItemDto updateDto, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetByIdAsync(listId, taskId, cancellationToken);
            if (task == null)
                throw new KeyNotFoundException("Task not found");

            if (task.IsCompleted)
                throw new InvalidOperationException("Completed tasks cannot be modified");

            _mapper.Map(updateDto, task);
            await _taskRepository.UpdateAsync(task, cancellationToken);
        }

        public async Task DeleteTaskAsync(int listId, int taskId, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetByIdAsync(listId, taskId, cancellationToken);
            if (task == null)
                throw new KeyNotFoundException("Task not found");

            await _taskRepository.DeleteAsync(task, cancellationToken);
        }

        public async Task MarkTaskAsDoneAsync(int listId, int taskId, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetByIdAsync(listId, taskId, cancellationToken);
            if (task == null)
                throw new KeyNotFoundException("Task not found");

            task.IsCompleted = true;
            await _taskRepository.UpdateAsync(task, cancellationToken);
        }

        public async Task<IEnumerable<TaskItemDto>> SearchTasksAsync(int? listId, string searchTerm, Priority? priorites, bool? isCompleted, CancellationToken cancellationToken = default)
        {
            var tasks = await _taskRepository.SearchAsync(listId, searchTerm, priorites, isCompleted, cancellationToken);
            return _mapper.Map<IEnumerable<TaskItemDto>>(tasks);
        }
    }
}
