using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskList.Application.Interfaces;
using TaskList.Domain.DTO;
using TaskList.Domain.RepositoryInerfaces;

namespace TaskList.Presentation.Controllers
{
    [ApiController]
    [Route("api/tasklists")]
    public class TaskListsController : ControllerBase
    {
        private readonly ITaskListApplicationService _taskListService;
        private readonly IMapper _mapper;

        public TaskListsController(ITaskListApplicationService taskListService, IMapper mapper)
        {
            _taskListService = taskListService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskListDto>>> GetAllTaskLists(CancellationToken cancellationToken)
        {
            var taskLists = await _taskListService.GetAllTaskListsAsync(cancellationToken);
            return Ok(taskLists);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskListDto>> GetTaskList(int id, CancellationToken cancellationToken)
        {
            var taskList = await _taskListService.GetTaskListByIdAsync(id, cancellationToken);
            return Ok(taskList);
        }

        [HttpPost]
        public async Task<ActionResult<TaskListDto>> CreateTaskList(CreateTaskListDto createDto, CancellationToken cancellationToken)
        {
            var taskList = await _taskListService.CreateTaskListAsync(createDto, cancellationToken);
            return CreatedAtAction(nameof(GetTaskList), new { id = taskList.Id }, taskList);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskList(int id, UpdateTaskListDto updateDto, CancellationToken cancellationToken)
        {
            await _taskListService.UpdateTaskListAsync(id, updateDto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskList(int id, CancellationToken cancellationToken)
        {
            await _taskListService.DeleteTaskListAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/image")]
        public async Task<ActionResult<ImageUploadResponse>> UploadImage(int id, [FromForm] UploadImageDto uploadDto, CancellationToken cancellationToken)
        {
            try
            {
                if (uploadDto?.Image == null)
                {
                    return BadRequest("No image file provided");
                }

                var imageUrl = await _taskListService.UpdateTaskListImageAsync(id, uploadDto.Image, cancellationToken);
                return Ok(new ImageUploadResponse { ImageUrl = imageUrl });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while uploading the image");
            }
        }

        [HttpDelete("{id}/image")]
        public async Task<IActionResult> RemoveImage(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _taskListService.RemoveTaskListImageAsync(id, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while removing the image");
            }
        }
    }
}

