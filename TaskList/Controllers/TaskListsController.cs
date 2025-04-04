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

        [HttpPost("{id}/image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ImageUploadResponse>> UploadImage(int id, [FromForm] UploadImageDto uploadDto)
        {
            try
            {
                if (uploadDto?.Image == null)
                {
                    return BadRequest("No image file provided");
                }

                var imageUrl = await _taskListService.UpdateTaskListImageAsync(id, uploadDto.Image);
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveImage(int id)
        {
            try
            {
                await _taskListService.RemoveTaskListImageAsync(id);
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
        //[HttpPost("{id}/image")]
        //public async Task<ActionResult<string>> UploadImage(int id, [FromForm] UploadImageDto uploadDto)
        //{
        //    var imageUrl = await _taskListService.UpdateTaskListImageAsync(id, uploadDto.Image);
        //    return Ok(new { ImageUrl = imageUrl });
        //}

        //[HttpDelete("{id}/image")]
        //public async Task<IActionResult> RemoveImage(int id)
        //{
        //    await _taskListService.RemoveTaskListImageAsync(id);
        //    return NoContent();
        //}
    }
}
