using System.ComponentModel.DataAnnotations;

namespace CSharp_teacher.Models
{
    public class TaskModel
    {
        public long Id { get; set; }
        public int SequenceNumber { get; set; }

        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Template { get; set; } = string.Empty;
        public string PublicTests { get; set; } = string.Empty;
        public string HiddenTests { get; set; } = string.Empty;
        public long ModuleId { get; set; }
        public Module Module { get; set; } = null!;
        public List<Hint> Hints { get; set; } = new();
        public List<Submission> Submissions { get; set; } = new();
        public List<UserTaskProgress> UserProgresses { get; set; } = new();
    }
}