using Finances.DTOs;
using Finances.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finances.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(AppDbContext context) : ControllerBase
{
    private static UserGetDto UserToDto(UserEntity user) =>
        new UserGetDto()
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Password = user.Password
        };

    /// <summary>
    /// Get all existing users
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<UserGetDto>>> Get()
    {
        return Ok(await context.Users
            .Select(x => UserToDto(x))
            .ToListAsync());
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserEntity), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserGetDto>> GetById(Guid id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return UserToDto(user);
    }

    /// <summary>
    /// Provide the users id to see detail info about this user
    /// </summary>
    /// <param name="id"></param>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserCreateDto>> Put(Guid id, UserCreateDto userDto)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null) return BadRequest();
        user.FullName = userDto.FullName;
        user.Email = userDto.Email;
        user.Password = userDto.Password;
        await context.SaveChangesAsync();
        return Ok(user);
    }

    /// <summary>
    /// Provide user id to update the information about the user.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserEntity>> Post(UserCreateDto userDto)
    {
        var user = new UserEntity
        {
            FullName = userDto.FullName,
            Email = userDto.Email,
            Password = userDto.Password
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>
    /// Deleting a user by its id
    /// </summary>
    /// <param name="id"></param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userDelete = await context.Users.FindAsync(id);
        if (userDelete == null) return NotFound();
        context.Users.Remove(userDelete);
        await context.SaveChangesAsync();
        return NoContent();
    }
}