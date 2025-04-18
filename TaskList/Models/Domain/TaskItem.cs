﻿using TaskList.Models.Enum;

namespace TaskList.Models.Domain
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DueDate { get; set; }

        public Priority Priorities { get; set; } = Priority.Medium;

        public bool IsDone { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        //////Navigation Properties//////
        public int TaskListId { get; set; }
        public TaskList TaskList { get; set; }


    }
}
