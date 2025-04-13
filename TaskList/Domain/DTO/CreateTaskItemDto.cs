using static TaskList.Domain.Entity.TaskItem;
using System.ComponentModel.DataAnnotations;

namespace TaskList.Domain.DTO
{
    public class CreateTaskItemDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public DateTime? DueDate { get; set; }
        public Priority Priorities { get; set; } = Priority.Medium;
    }
}
