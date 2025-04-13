using static TaskList.Models.Domain.TaskItem;
using System.ComponentModel.DataAnnotations;
using TaskList.Models.Enum;

namespace TaskList.Models.DTO
{
    public class UpdateTaskItemDto
    {
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        [Range(0, 2, ErrorMessage = "Priority must be between 0 (Low) and 2 (High)")]
        public Priority? Priorities { get; set; }
    }
}
