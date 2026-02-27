using FluentAssertions;
using Microsoft.Extensions.Configuration;
using ProductApi.DTOs;
using ProductApi.Services;
using Xunit;

namespace ProductApi.Tests;

public class AuthServiceTests
{
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "ThisIsASecretKeyForJwtTokenGeneration2024!",
                ["Jwt:Issuer"] = "ProductApi",
                ["Jwt:Audience"] = "ProductFrontend"
            })
            .Build();

        _authService = new AuthService(config);
    }

    [Fact]
    public void Login_ValidCredentials_ShouldReturnToken()
    {
        LoginResponse result = _authService.Login(new LoginRequest("admin", "Admin@1234"));

        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.Username.Should().Be("admin");
    }

    [Fact]
    public void Login_WrongPassword_ShouldReturnNull()
    {
        LoginResponse? result = _authService.Login(new LoginRequest("admin", "wrongpassword"));
        result.Should().BeNull();
    }

    [Fact]
    public void Login_UnknownUser_ShouldReturnNull()
    {
        LoginResponse? result = _authService.Login(new LoginRequest("unknown", "Admin@1234"));
        result.Should().BeNull();
    }

    [Fact]
    public void Login_CaseInsensitiveUsername_ShouldReturnToken()
    {
        LoginResponse? result = _authService.Login(new LoginRequest("ADMIN", "Admin@1234"));
        result.Should().NotBeNull();
    }

    [Fact]
    public void Login_User1_ShouldReturnToken()
    {

        LoginResponse? result = _authService.Login(new LoginRequest("user1", "User@1234"));

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("user1");
    }
}
