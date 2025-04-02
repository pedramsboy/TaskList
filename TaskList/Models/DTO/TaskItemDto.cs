﻿using static TaskList.Models.Domain.TaskItem;

namespace TaskList.Models.DTO
{
    public class TaskItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public Priority Priority { get; set; }
        public bool IsCompleted { get; set; }
        public int TaskListId { get; set; }
    }
}
