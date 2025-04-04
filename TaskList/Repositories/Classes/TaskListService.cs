using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using TaskList.Data;
using TaskList.Models.DTO;
using TaskList.Repositories.Interfaces;

namespace TaskList.Repositories.Classes
{
    public class TaskListService : ITaskListService
    {
        private readonly TaskDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<FileStorageService> _logger;
        public TaskListService(TaskDbContext context, IMapper mapper, IFileStorageService fileStorageService, ILogger<FileStorageService> logger)
        {
            _context = context;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskListDto>> GetAllTaskListsAsync()
        {
            var taskLists = await _context.TaskLists
                .Where(tl => !tl.IsDeleted)
                .Include(tl => tl.Tasks)
                .ToListAsync();

            var taskListDtos = _mapper.Map<IEnumerable<TaskListDto>>(taskLists);

            foreach (var dto in taskListDtos)
            {
                var taskList = taskLists.First(tl => tl.Id == dto.Id);
                dto.ImageUrl = _fileStorageService.GetFileUrl(taskList.ImagePath, "tasklists");
            }

            return taskListDtos;
        }

        public async Task<TaskListDto> GetTaskListByIdAsync(int id)
        {
            var taskList = await _context.TaskLists
                .Include(tl => tl.Tasks)
                .FirstOrDefaultAsync(tl => tl.Id == id && !tl.IsDeleted);

            if (taskList == null)
                throw new KeyNotFoundException("Task list not found");

            var dto = _mapper.Map<TaskListDto>(taskList);
            dto.ImageUrl = _fileStorageService.GetFileUrl(taskList.ImagePath, "tasklists");
            return dto;
        }

        public async Task<TaskListDto> CreateTaskListAsync(CreateTaskListDto createDto)
        {
            var taskList = _mapper.Map<TaskList.Models.Domain.TaskList>(createDto);
            _context.TaskLists.Add(taskList);
            await _context.SaveChangesAsync();
            return _mapper.Map<TaskListDto>(taskList);
        }

        public async Task UpdateTaskListAsync(int id, UpdateTaskListDto updateDto)
        {
            var taskList = await _context.TaskLists.FindAsync(id);
            if (taskList == null || taskList.IsDeleted)
                throw new KeyNotFoundException("Task list not found");

            _mapper.Map(updateDto, taskList);
            taskList.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskListAsync(int id)
        {
            var taskList = await _context.TaskLists
                .Include(tl => tl.Tasks)
                .FirstOrDefaultAsync(tl => tl.Id == id && !tl.IsDeleted);

            if (taskList == null)
                throw new KeyNotFoundException("Task list not found");

            taskList.IsDeleted = true;
            taskList.UpdatedAt = DateTime.UtcNow;

            foreach (var task in taskList.Tasks)
            {
                task.IsDeleted = true;
                task.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
        public async Task<string> UpdateTaskListImageAsync(int id, IFormFile imageFile)
        {
            var taskList = await _context.TaskLists.FindAsync(id);
            if (taskList == null || taskList.IsDeleted)
            {
                throw new KeyNotFoundException("Task list not found");
            }

            try
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(taskList.ImagePath))
                {
                    await _fileStorageService.DeleteFileAsync(taskList.ImagePath, "tasklists");
                }

                // Save new image
                var imagePath = await _fileStorageService.SaveFileAsync(imageFile, "tasklists");
                taskList.ImagePath = imagePath;
                taskList.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return _fileStorageService.GetFileUrl(imagePath, "tasklists");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task list image");
                throw; // Re-throw for controller to handle
            }
        }

        public async Task RemoveTaskListImageAsync(int id)
        {
            var taskList = await _context.TaskLists.FindAsync(id);
            if (taskList == null || taskList.IsDeleted)
            {
                throw new KeyNotFoundException("Task list not found");
            }

            if (!string.IsNullOrEmpty(taskList.ImagePath))
            {
                try
                {
                    await _fileStorageService.DeleteFileAsync(taskList.ImagePath, "tasklists");
                    taskList.ImagePath = null;
                    taskList.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error removing task list image");
                    throw; // Re-throw for controller to handle
                }
            }
        }

    }
}
