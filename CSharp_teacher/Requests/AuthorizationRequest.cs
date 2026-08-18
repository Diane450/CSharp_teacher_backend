using System.Diagnostics;

namespace CSharp_teacher.Requests
{
    public class AuthorizationRequest
    {
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
