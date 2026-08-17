namespace CSharp_teacher.Models
{
    public class OpenedHints
    {
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public long HintId { get; set; }
        public Hint Hint { get; set; } = null!;

    }
}
