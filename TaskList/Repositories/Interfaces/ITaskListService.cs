﻿using TaskList.Models.DTO;

namespace TaskList.Repositories.Interfaces
{
    public interface ITaskListService
    {
        Task<IEnumerable<TaskListDto>> GetAllTaskListsAsync(CancellationToken cancellationToken = default);
        Task<TaskListDto> GetTaskListByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TaskListDto> CreateTaskListAsync(CreateTaskListDto createDto, CancellationToken cancellationToken = default);
        Task UpdateTaskListAsync(int id, UpdateTaskListDto updateDto, CancellationToken cancellationToken = default);
        Task DeleteTaskListAsync(int id, CancellationToken cancellationToken = default);
        Task<string> UpdateTaskListImageAsync(int id, IFormFile imageFile, CancellationToken cancellationToken = default);
        Task RemoveTaskListImageAsync(int id, CancellationToken cancellationToken = default);
    }
}
