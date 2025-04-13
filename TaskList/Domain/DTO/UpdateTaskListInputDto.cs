using System.ComponentModel.DataAnnotations;

namespace TaskList.Domain.DTO
{
    public class UpdateTaskListInputDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
