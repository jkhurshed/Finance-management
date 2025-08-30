using Finances.DTOs;
using Finances.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finances.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;
    public UserController(AppDbContext context)
    {
        _context = context;
    }
    private static UserGetDto UserToDto(UserEntity user) =>
        new UserGetDto()
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Password = user.Password
        };

    [HttpGet]
    public async Task<IEnumerable<UserGetDto>> Get()
    {
        return await _context.Users
            .Select(x => UserToDto(x))
            .ToListAsync();
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserEntity), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserGetDto>> GetById(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return UserToDto(user);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserCreateDto>> Put(Guid id, UserCreateDto userDto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return BadRequest();
        user.FullName = userDto.FullName;
        user.Email = userDto.Email;
        user.Password = userDto.Password;
        await _context.SaveChangesAsync();
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserEntity>> Post(UserCreateDto userDto)
    {
        var user = new UserEntity
        {
            FullName = userDto.FullName,
            Email = userDto.Email,
            Password = userDto.Password
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var userDelete = await _context.Users.FindAsync(id);
        if (userDelete == null) return NotFound();
        _context.Users.Remove(userDelete);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}