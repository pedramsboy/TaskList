namespace TaskList.Models.DTO
{
    public class TaskListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TaskCount { get; set; }
        public string? ImageUrl { get; set; }
    }
}
