using CSharp_teacher.Data;
using CSharp_teacher.DTO;
using CSharp_teacher.Models;
using CSharp_teacher.Requests;
using CSharp_teacher.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSharp_teacher.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly TokenService _tokenService;
        private readonly AppDbContext _context;
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
            _context = appDbContext;
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
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshTokenString = _tokenService.GenerateRefreshToken();
            var refreshTokenDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"]!);

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays),
                IsRevoked = false,
            };
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
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
                message = "Успешная регистрация",
                accessToken = accessToken,
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthorizationRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Login);
            if (user == null)
            {
                return Unauthorized(new { message = "Неверный логин или пароль" });
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                return StatusCode(403, new { message = "Аккаунт временно заблокирован из-за 5 неудачных попыток. Попробуйте через 5 минут." });
            }

            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Неверный логин или пароль" });
            }
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshTokenString = _tokenService.GenerateRefreshToken();
            var refreshTokenDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"]!);

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays),
                IsRevoked = false,
            };
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
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

        [HttpPost("logout")]
        public async Task<IActionResult> LogOut()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshTokenString))
            {
                return Ok(new { message = "Вы уже вышли из системы"});
            }

            var existingToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshTokenString);

            if (existingToken != null)
            {
                existingToken.IsRevoked = true;
                existingToken.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            Response.Cookies.Delete("refreshToken");
            return Ok(new { message = "Вы успешно вышли из системы" });
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok();
        }
    }
}
