namespace CSharp_teacher.Models
{
    public class SubmissionResult
    {
        public SubmissionStatus Status { get; set; }
        public double ExecutionTimeMs { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
