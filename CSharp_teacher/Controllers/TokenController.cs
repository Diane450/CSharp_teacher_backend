using CSharp_teacher.Data;
using CSharp_teacher.Models;
using CSharp_teacher.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSharp_teacher.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly TokenService _tokenService = null!;
        private readonly AppDbContext _context = null!;
        private readonly IConfiguration _configuration = null!;
        public TokenController(TokenService tokenService, AppDbContext context, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("create-new-tokens")]
        public async Task<IActionResult> CreateNewTokens()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshTokenString))
            {
                return Unauthorized("Отсутствует RefreshToken");
            }
            var existingRefreshToken = await _context.RefreshTokens
                                            .Include(rt => rt.User)
                                            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenString);
            if (existingRefreshToken == null || existingRefreshToken!.IsRevoked || existingRefreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                return Unauthorized();
            }

            var user = existingRefreshToken.User;
            existingRefreshToken.IsRevoked = true;
            existingRefreshToken.RevokedAt = DateTime.UtcNow;

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshTokenString = _tokenService.GenerateRefreshToken();
            var newRefreshTokenDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"]!);

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(newRefreshTokenDays),
                IsRevoked = false,
            };
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(newRefreshTokenDays),
            };
            Response.Cookies.Append("refreshToken", refreshTokenString, cookieOptions);
            return Ok(new
            {
                message = "Успешный вход",
                accessToken = newAccessToken,
            });
        }

        [HttpGet("create-access-token")]
        public async Task<IActionResult> CreateNewAccessToken()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshTokenString))
            {
                return Unauthorized();
            }

            var existingToken = await _context.RefreshTokens
                                .Include(rt => rt.User)
                                .FirstOrDefaultAsync(rt => rt.Token == refreshTokenString);

            if (existingToken == null || existingToken.IsRevoked || existingToken.ExpiresAt <= DateTime.UtcNow)
            {
                return Unauthorized();
            }
            var token = _tokenService.GenerateAccessToken(existingToken.User);
            return Ok(new { accessToken = token });
        }
    }
}