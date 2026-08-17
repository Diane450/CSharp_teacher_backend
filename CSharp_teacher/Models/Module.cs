using System.ComponentModel.DataAnnotations;

namespace CSharp_teacher.Models
{
    public class Module
    {
        public long Id { get; set; }
        public int SequenceNumber { get; set; }

        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        public long CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public List<TaskModel> Tasks { get; set; } = new();
    }
}
