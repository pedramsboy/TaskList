using AutoMapper;
using TaskList.Application.Interfaces;
using TaskList.Domain.DTO;
using TaskList.Domain.RepositoryInerfaces;
using TaskList.Repositories.Interfaces;

namespace TaskList.Application.Classes
{
    public class TaskListService : ITaskListService
    {
        private readonly ITaskListRepository _repository;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<TaskListService> _logger;

        public TaskListService(
            ITaskListRepository repository,
            IMapper mapper,
            IFileStorageService fileStorageService,
            ILogger<TaskListService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskListDto>> GetAllTaskListsAsync(CancellationToken cancellationToken = default)
        {
            var taskLists = await _repository.GetAllAsync(cancellationToken);
            var taskListDtos = _mapper.Map<IEnumerable<TaskListDto>>(taskLists);

            foreach (var dto in taskListDtos)
            {
                var taskList = taskLists.First(tl => tl.Id == dto.Id);
                dto.ImageUrl = _fileStorageService.GetFileUrl(taskList.ImagePath, "tasklists");
            }

            return taskListDtos;
        }

        public async Task<TaskListDto> GetTaskListByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var taskList = await _repository.GetByIdAsync(id, cancellationToken);
            if (taskList == null)
                throw new KeyNotFoundException("Task list not found");

            var dto = _mapper.Map<TaskListDto>(taskList);
            dto.ImageUrl = _fileStorageService.GetFileUrl(taskList.ImagePath, "tasklists");
            return dto;
        }

        public async Task<TaskListDto> CreateTaskListAsync(CreateTaskListDto createDto, CancellationToken cancellationToken = default)
        {
            var taskList = _mapper.Map<TaskList.Domain.Entity.TaskList>(createDto);
            taskList = await _repository.AddAsync(taskList, cancellationToken);
            return _mapper.Map<TaskListDto>(taskList);
        }

        public async Task UpdateTaskListAsync(int id, UpdateTaskListDto updateDto, CancellationToken cancellationToken = default)
        {
            var taskList = await _repository.GetByIdAsync(id, cancellationToken);
            if (taskList == null)
                throw new KeyNotFoundException("Task list not found");

            _mapper.Map(updateDto, taskList);
            await _repository.UpdateAsync(taskList, cancellationToken);
        }

        public async Task DeleteTaskListAsync(int id, CancellationToken cancellationToken = default)
        {
            var taskList = await _repository.GetByIdAsync(id, cancellationToken);
            if (taskList == null)
                throw new KeyNotFoundException("Task list not found");

            await _repository.DeleteAsync(taskList, cancellationToken);
        }

        public async Task<string> UpdateTaskListImageAsync(int id, IFormFile imageFile, CancellationToken cancellationToken = default)
        {
            var taskList = await _repository.GetByIdAsync(id, cancellationToken);
            if (taskList == null)
                throw new KeyNotFoundException("Task list not found");

            try
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(taskList.ImagePath))
                {
                    await _fileStorageService.DeleteFileAsync(taskList.ImagePath, "tasklists", cancellationToken);
                }

                // Save new image
                var imagePath = await _fileStorageService.SaveFileAsync(imageFile, "tasklists", cancellationToken);
                taskList.ImagePath = imagePath;
                await _repository.UpdateAsync(taskList, cancellationToken);

                return _fileStorageService.GetFileUrl(imagePath, "tasklists");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task list image");
                throw;
            }
        }

        public async Task RemoveTaskListImageAsync(int id, CancellationToken cancellationToken = default)
        {
            var taskList = await _repository.GetByIdAsync(id, cancellationToken);
            if (taskList == null)
                throw new KeyNotFoundException("Task list not found");

            if (!string.IsNullOrEmpty(taskList.ImagePath))
            {
                try
                {
                    await _fileStorageService.DeleteFileAsync(taskList.ImagePath, "tasklists", cancellationToken);
                    taskList.ImagePath = null;
                    await _repository.UpdateAsync(taskList, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error removing task list image");
                    throw;
                }
            }
        }
    }
}
