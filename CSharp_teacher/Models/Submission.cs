namespace CSharp_teacher.Models
{
    public class Submission
    {
        public long Id { get; set; }
        public long TaskId { get; set; }
        public TaskModel Task { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public string SentCode { get; set; } = string.Empty;
        public SubmissionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public SubmissionResult Result { get; set; } = null!;
    }
}