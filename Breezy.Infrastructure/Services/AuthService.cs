using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Breezy.Application.Interfaces;
using Breezy.Domain.Users.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Breezy.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    public AuthService(IConfiguration configuration)
    {
        _jwtSecret = configuration["Jwt:Secret"];
        _jwtIssuer = configuration["Jwt:Issuer"];
        _jwtAudience = configuration["Jwt:Audience"];
    }

    public async Task<string> GenerateTokenAsync(User user)
    {
        var key = Encoding.UTF8.GetBytes(_jwtSecret);

        var claims = new[]
        {
            new Claim("sub", user.Id.ToString()),
            new Claim("email", user.Email.ToString()),
            new Claim("role", user.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _jwtIssuer,
            Audience = _jwtAudience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<Guid?> ValidateTokenAsync(string token)
    {
        var key = Encoding.UTF8.GetBytes(_jwtSecret);
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,      
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,              
            ValidIssuer = _jwtIssuer,
            ValidateAudience = true,           
            ValidAudience = _jwtAudience,
            ValidateLifetime = true               
        };
        
        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            var userId = principal.FindFirst("sub")?.Value;
            return userId != null ? Guid.Parse(userId) : null;
        }
        catch
        {
            return null;
        };
    }

    public async Task<string> HashPasswordAsync(string password)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(password);

        return await Task.FromResult(hash);
    }

    public async Task<bool> VerifyPasswordAsync(string password, string passwordHash)
    {
        return await Task.FromResult(BCrypt.Net.BCrypt.Verify(password, passwordHash));
    }
}