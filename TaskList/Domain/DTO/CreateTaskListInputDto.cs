using System.ComponentModel.DataAnnotations;

namespace TaskList.Domain.DTO
{
    public class CreateTaskListInputDto
    {
        public class CreateTaskListDto
        {
            [Required]
            [MaxLength(100)]
            public string Name { get; set; }
        }
    }
}
