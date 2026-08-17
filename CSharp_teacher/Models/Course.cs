using System.ComponentModel.DataAnnotations;

namespace CSharp_teacher.Models
{
    public class Course
    {
        public long Id { get; set; }
        public int SequenceNumber { get; set; }

        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<Module> Modules { get; set; } = new();
    }
}
