namespace CSharp_teacher.Models
{
    public class UserTaskProgress
    {
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public long TaskId { get; set; }
        public TaskModel Task { get; set; } = null!;
        public string? DraftCode { get; set; }
    }
}
