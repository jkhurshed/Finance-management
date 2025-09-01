using System.Net;
using Finances.DTOs;
using Finances.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finances.Controllers;

[ApiController]
[Route("[controller]")]
public class WalletController : ControllerBase
{
    private readonly AppDbContext _context;

    public WalletController(AppDbContext context)
    {
        _context = context;
    }

    private static WalletGetDto WalletToDto(WalletEntity wallet) =>
        new WalletGetDto()
        {
            Id = wallet.Id,
            Title = wallet.Title,
            Description = wallet.Description,
            AccountType = wallet.AccountType,
            Balance = wallet.Balance,
            Currency = wallet.Currency,
            UserId = wallet.UserId
        };

    [HttpGet]
    public async Task<IEnumerable<WalletGetDto>> Get()
    {
        return await _context.Wallets
            .Select(x => WalletToDto(x))
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WalletGetDto>> GetById(Guid id)
    {
        var wallet = await _context.Wallets.FindAsync(id);
        if (wallet == null) return NotFound();
        return WalletToDto(wallet);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<WalletCreateDto>> Put(Guid id, WalletCreateDto wallet)
    {
        var user = await _context.Wallets.FindAsync(id);
        if (wallet == null) return BadRequest();
        user.Title = wallet.Title;
        user.Description = wallet.Description;
        user.AccountType = wallet.AccountType;
        user.Balance = wallet.Balance;
        user.Currency = wallet.Currency;
        user.UserId = wallet.UserId;
        await _context.SaveChangesAsync();
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<WalletEntity>> Post(WalletCreateDto walletDto)
    {
        var user = await _context.Users.FindAsync(walletDto.UserId);
        if (user == null) return NotFound("User not found!");
        var wallet = new WalletEntity
        {
            Title = walletDto.Title,
            Description = walletDto.Description,
            AccountType = walletDto.AccountType,
            Balance = walletDto.Balance,
            Currency = walletDto.Currency,
            UserId = walletDto.UserId
        };
        await _context.Wallets.AddAsync(wallet);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = wallet.Id }, wallet);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var wallet = await _context.Wallets.FindAsync(id);
        if (wallet == null) return NotFound();
        _context.Wallets.Remove(wallet);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}