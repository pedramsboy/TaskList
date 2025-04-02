using static TaskList.Models.Domain.TaskItem;
using System.ComponentModel.DataAnnotations;

namespace TaskList.Models.DTO
{
    public class UpdateTaskItemDto
    {
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public DateTime? DueDate { get; set; }
        public Priority? Priorities { get; set; }
    }
}
