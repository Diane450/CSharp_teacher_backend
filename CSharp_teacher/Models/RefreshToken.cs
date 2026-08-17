namespace CSharp_teacher.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public bool IsRevoked { get; set; }
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
    }
}
