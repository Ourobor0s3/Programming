namespace TaskManager
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; } = DateTime.UtcNow;
        public bool IsDone { get; set; }
        
    }
}