using CSharp_teacher.Data;
using CSharp_teacher.DTO;
using CSharp_teacher.Models;
using CSharp_teacher.Requests;
using CSharp_teacher.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CSharp_teacher.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly TokenService _tokenService;
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;
        public AuthorizationController(UserManager<User> userManager,
                                        SignInManager<User> signInManager,
                                        TokenService tokenService,
                                        AppDbContext appDbContext,
                                        IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _configuration = configuration;
            _appDbContext = appDbContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest registrationRequest)
        {
            User user = new User()
            {
                UserName = registrationRequest.Login,
                Email = registrationRequest.Email,
            };
            var result = await _userManager.CreateAsync(user, registrationRequest.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { message = "Пользователь успешно зарегистрирован!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthorizationRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Неверная почта или пароль" });
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Неверная почта или пароль" });
            }
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshTokenString = _tokenService.GenerateRefreshToken();
            var refreshTokenDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"]!);

            var refreshToken = new RefreshToken
            {
                Token = accessToken,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays),
                IsRevoked = false,
            };
            _appDbContext.RefreshTokens.Add(refreshToken);
            await _appDbContext.SaveChangesAsync();
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(refreshTokenDays),
            };
            Response.Cookies.Append("refreshToken", refreshTokenString, cookieOptions);
            return Ok(new
            {
                message = "Успешный вход",
                accessToken = accessToken,
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult Test()
        {
            return Ok(new { message = "Ok" });
        }
    }
}
