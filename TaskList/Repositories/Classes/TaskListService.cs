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

        public TaskListService(TaskDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskListDto>> GetAllTaskListsAsync()
        {
            var taskLists = await _context.TaskLists
                .Where(tl => !tl.IsDeleted)
                .Include(tl => tl.Tasks)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TaskListDto>>(taskLists);
        }

        public async Task<TaskListDto> GetTaskListByIdAsync(int id)
        {
            var taskList = await _context.TaskLists
                .Include(tl => tl.Tasks)
                .FirstOrDefaultAsync(tl => tl.Id == id && !tl.IsDeleted);

            if (taskList == null)
                throw new KeyNotFoundException("Task list not found");

            return _mapper.Map<TaskListDto>(taskList);
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
    }
}
