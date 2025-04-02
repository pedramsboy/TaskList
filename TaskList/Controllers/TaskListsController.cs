using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskList.Models.DTO;
using TaskList.Repositories.Interfaces;

namespace TaskList.Controllers
{
    [ApiController]
    [Route("api/tasklists")]
    public class TaskListsController : ControllerBase
    {
        private readonly ITaskListService _taskListService;
        private readonly IMapper _mapper;

        public TaskListsController(ITaskListService taskListService, IMapper mapper)
        {
            _taskListService = taskListService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskListDto>>> GetAllTaskLists()
        {
            var taskLists = await _taskListService.GetAllTaskListsAsync();
            return Ok(taskLists);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskListDto>> GetTaskList(int id)
        {
            var taskList = await _taskListService.GetTaskListByIdAsync(id);
            return Ok(taskList);
        }

        [HttpPost]
        public async Task<ActionResult<TaskListDto>> CreateTaskList(CreateTaskListDto createDto)
        {
            var taskList = await _taskListService.CreateTaskListAsync(createDto);
            return CreatedAtAction(nameof(GetTaskList), new { id = taskList.Id }, taskList);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskList(int id, UpdateTaskListDto updateDto)
        {
            await _taskListService.UpdateTaskListAsync(id, updateDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskList(int id)
        {
            await _taskListService.DeleteTaskListAsync(id);
            return NoContent();
        }
    }
}
