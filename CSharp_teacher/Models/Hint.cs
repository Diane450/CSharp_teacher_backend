namespace CSharp_teacher.Models
{
    public class Hint
    {
        public long Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public long TaskId { get; set; }
        public TaskModel Task { get; set; } = null!;
        public List<OpenedHints> OpenedHints { get; set; } = new();

    }
}