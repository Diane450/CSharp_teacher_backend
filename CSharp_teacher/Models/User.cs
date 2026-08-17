using Microsoft.AspNetCore.Identity;

namespace CSharp_teacher.Models
{
    public class User : IdentityUser
    {
        public List<UserTaskProgress> TaskProgresses { get; set; } = new();
        public List<OpenedHints> OpenedHints { get; set; } = new();
        public List<Submission> Submissions { get; set; } = new();
        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }
}
