using System.ComponentModel.DataAnnotations;

namespace TaskList.Models.DTO
{
    public class UpdateTaskListDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
