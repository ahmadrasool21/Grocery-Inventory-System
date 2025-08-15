using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GroceryInventory.Api.Auth;

public record LoginRequest(string Username, string Password);
public record TokenResponse(string AccessToken, DateTime ExpiresAtUtc, string Role);

[ApiController]
[Route("api/auth")]
public class AuthController(IOptions<JwtSettings> jwtOptions) : ControllerBase
{
    // Demo users (replace with real user store later)
    private static readonly Dictionary<string, (string Password, string Role)> Users = new()
    {
        ["admin"] = ("Admin123!", "Admin"),
        ["clerk"] = ("Clerk123!", "Clerk")
    };

    [AllowAnonymous]
    [HttpPost("token")]
    public IActionResult Token([FromBody] LoginRequest req)
    {
        if (!Users.TryGetValue(req.Username, out var user) || user.Password != req.Password)
            return Unauthorized(new { message = "Invalid credentials" });

        var settings = jwtOptions.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(8);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, req.Username),
            new(ClaimTypes.Name, req.Username),
            new(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new TokenResponse(jwt, expires, user.Role));
    }
}