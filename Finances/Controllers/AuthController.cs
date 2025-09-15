using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Finances.DTOs;
using Finances.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Finances.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class AuthController(UserManager<UserEntity> userManager, IConfiguration config) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new UserEntity
        {
            FullName = dto.FullName,
            Email = dto.Email,
            UserName = dto.FullName,
            PasswordHash = dto.Password
        };
        var result = await userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "User"); // default role
            return Ok("User registered successfully");
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, dto.Password))
            return Unauthorized();

        var token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    private string GenerateJwtToken(UserEntity user)
    {
        var jwt = config.GetSection("Jwt");
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwt["ExpireMinutes"])),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}