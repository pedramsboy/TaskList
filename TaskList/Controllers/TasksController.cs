using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static TaskList.Models.Domain.TaskItem;
using TaskList.Models.DTO;
using TaskList.Repositories.Interfaces;

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
            [FromQuery] bool isAscending = true)
        {
            var tasks = await _taskService.GetTasksByListIdAsync(listId, sortBy, isAscending);
            return Ok(tasks);
        }

        [HttpGet("{taskId}")]
        public async Task<ActionResult<TaskItemDto>> GetTask(int listId, int taskId)
        {
            var task = await _taskService.GetTaskByIdAsync(listId, taskId);
            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItemDto>> CreateTask(int listId, CreateTaskItemDto createDto)
        {
            var task = await _taskService.CreateTaskAsync(listId, createDto);
            return CreatedAtAction(nameof(GetTask), new { listId, taskId = task.Id }, task);
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(int listId, int taskId, UpdateTaskItemDto updateDto)
        {
            await _taskService.UpdateTaskAsync(listId, taskId, updateDto);
            return NoContent();
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(int listId, int taskId)
        {
            await _taskService.DeleteTaskAsync(listId, taskId);
            return NoContent();
        }

        [HttpPatch("{taskId}/complete")]
        public async Task<IActionResult> MarkTaskAsDone(int listId, int taskId)
        {
            await _taskService.MarkTaskAsDoneAsync(listId, taskId);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> SearchTasks(
            int? listId,
            [FromQuery] string searchTerm = null,
            [FromQuery] Priority? priority = null,
            [FromQuery] bool? isCompleted = null)
        {
            var tasks = await _taskService.SearchTasksAsync(listId, searchTerm, priority, isCompleted);
            return Ok(tasks);
        }
    }
}
