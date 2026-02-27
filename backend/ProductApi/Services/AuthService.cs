using Microsoft.IdentityModel.Tokens;
using ProductApi.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductApi.Services;

public interface IAuthService
{
    LoginResponse? Login(LoginRequest request);
}

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;

    // Hardcoded users (username: password)
    private static readonly Dictionary<string, string> Users = new(StringComparer.OrdinalIgnoreCase)
    {
        { "admin", "Admin@1234" },
        { "user1", "User@1234" }
    };

    public AuthService(IConfiguration config)
    {
        _config = config;
    }

    public LoginResponse? Login(LoginRequest request)
    {
        if (!Users.TryGetValue(request.Username, out var password))
            return null;

        if (password != request.Password)
            return null;

        string token = GenerateToken(request.Username);
        return new LoginResponse(token, request.Username);
    }

    private string GenerateToken(string username)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        Claim[] claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "User"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
