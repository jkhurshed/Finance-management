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
    
    /// <summary>
    /// Provide user id to see detail info about this user
    /// </summary>
    /// <param name="id"></param>
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
        
    /// <summary>
    /// Get the user's transactions
    /// </summary>
    /// <param name="id"></param>
    [HttpGet("UserTransactions/{id}")]
    public async Task<ActionResult<TransactionEntity>> GetUsersTransactions(Guid id)
    {
        var transaction = await (
            from t in context.Transactions
            join w in context.Wallets on t.WalletId equals w.Id
            join u in context.Users on w.UserId equals u.Id
            where u.Id == id
            select new UserTransactionsGetDto()
            {
                UserId = t.Id,
                FullName = u.FullName,
                TransactionTitle = t.Title,
                Description = t.Description,
                TransactionAmount = t.Amount,
                TransactionType = t.TransactionType,
                WalletTitle = w.Title,
                CategoryId = t.CategoryId,
                Date = t.CreatedAt
            }
        ).ToListAsync();
        
        return Ok(transaction);
    }

    /// <summary>
    /// Get the user's total balance
    /// </summary>
    /// <param name="id"></param>
    [HttpGet("UserTotal/{id}")]
    public async Task<ActionResult<WalletEntity>> GetUsersTotalBalance(Guid id)
    {
        var totalBalance = await context.Wallets
            .Include(x=>x.User)
            .Where(w => w.UserId == id)
            .GroupBy(w => w.UserId)
            .Select(w => new UsersTotalAmountGetDto()
                {
                    UserId = w.Key,
                    FullName = w.First().User.FullName,
                    TotalAmount = w.Sum(t => t.Balance)
                }
            )
            .ToListAsync();
        
        return Ok(totalBalance);
    }
}