using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static TaskList.Domain.Entity.TaskItem;
using TaskList.Domain.DTO;
using TaskList.Domain.RepositoryInerfaces;
using TaskList.Application.Interfaces;

namespace TaskList.Controllers
{
    [ApiController]
    [Route("api/tasklists/{listId}/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;

        public TasksController(ITaskService taskService, IMapper mapper)
        {
            _taskService = taskService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTasks(int listId,
            [FromQuery] string sortBy = "id",
            [FromQuery] bool isAscending = true,
            CancellationToken cancellationToken = default)
        {

            var tasks = await _taskService.GetTasksByListIdAsync(listId, sortBy, isAscending, cancellationToken);
            return Ok(tasks);

        }

        [HttpGet("{taskId}")]
        public async Task<ActionResult<TaskItemDto>> GetTask(int listId, int taskId, CancellationToken cancellationToken = default)
        {

            var task = await _taskService.GetTaskByIdAsync(listId, taskId, cancellationToken);
            return Ok(task);

        }

        [HttpPost]
        public async Task<ActionResult<TaskItemDto>> CreateTask(int listId, CreateTaskItemDto createDto, CancellationToken cancellationToken = default)
        {

            var task = await _taskService.CreateTaskAsync(listId, createDto, cancellationToken);
            return CreatedAtAction(nameof(GetTask), new { listId, taskId = task.Id }, task);

        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(int listId, int taskId, UpdateTaskItemDto updateDto, CancellationToken cancellationToken = default)
        {

            await _taskService.UpdateTaskAsync(listId, taskId, updateDto, cancellationToken);
            return NoContent();

        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(int listId, int taskId, CancellationToken cancellationToken = default)
        {

            await _taskService.DeleteTaskAsync(listId, taskId, cancellationToken);
            return NoContent();

        }

        [HttpPatch("{taskId}/complete")]
        public async Task<IActionResult> MarkTaskAsDone(int listId, int taskId, CancellationToken cancellationToken = default)
        {

            await _taskService.MarkTaskAsDoneAsync(listId, taskId, cancellationToken);
            return NoContent();

        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> SearchTasks(
            int? listId,
            [FromQuery] string searchTerm = null,
            [FromQuery] Priority? priority = null,
            [FromQuery] bool? isCompleted = null,
            CancellationToken cancellationToken = default)
        {

            var tasks = await _taskService.SearchTasksAsync(listId, searchTerm, priority, isCompleted, cancellationToken);
            return Ok(tasks);
        }
    }
}
