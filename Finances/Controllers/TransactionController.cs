using Finances.DTOs;
using Finances.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finances.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionController : ControllerBase
{
    private readonly AppDbContext _context;

    public TransactionController(AppDbContext context)
    {
        _context = context;
    }

    private static TransactionGetDto TransactionToDto(TransactionEntity transaction, string categoryTitle) =>
        new TransactionGetDto()
        {
            Id = transaction.Id,
            Title = transaction.Title,
            Description = transaction.Description,
            Amount = transaction.Amount,
            TransactionType = transaction.TransactionType,
            IsActive = transaction.IsActive,
            WalletId = transaction.WalletId,
            CategoryId = transaction.CategoryId,
            Date = transaction.CreatedAt,
            CategoryTitle = categoryTitle
        };
    
    [HttpGet]
    public async Task<IEnumerable<TransactionGetDto>> Get()
    {
        var transactions = await _context.Transactions.ToListAsync();
        var categories = await _context.Categories
            .ToDictionaryAsync(x => x.Id, x => x.Title);
        return transactions.Select(t =>
            TransactionToDto(t, categories.GetValueOrDefault(t.CategoryId))
        ).ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionGetDto>> GetById(Guid id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        var categories = await _context.Categories
            .ToDictionaryAsync(x => x.Id, x => x.Title);
        if (transaction == null)
        {
            return NotFound();
        }
        return TransactionToDto(transaction, categories.GetValueOrDefault(transaction.CategoryId));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TransactionCreateDto>> Put(Guid id, TransactionCreateDto transactionDto)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction == null) return NotFound();
        if (transactionDto.Amount <= 0)
        {
            return  BadRequest("Amount is less then wallet amount!");
        }
        transaction.Title = transactionDto.Title;
        transaction.Amount = transactionDto.Amount;
        transaction.TransactionType = transactionDto.Type;
        transaction.CategoryId = transactionDto.CategoryId;
        await _context.SaveChangesAsync();
        return Ok(transaction);
    }

    [HttpPost]
    public async Task<ActionResult<TransactionEntity>> Post(TransactionCreateDto transactionDto)
    {
        var wallet = await _context.Wallets.FindAsync(transactionDto.WalletId);
        if (wallet == null) return NotFound();
        
        if (transactionDto.Amount <= 0)
        {
            return BadRequest("Amount is less then wallet amount!");
        }

        if (transactionDto.Amount > wallet.Balance)
        {
            return BadRequest("Amount is more then wallet amount!");
        }

        var transaction = new TransactionEntity()
        {
            Title = transactionDto.Title,
            Amount = transactionDto.Amount,
            TransactionType = transactionDto.Type,
            IsActive = transactionDto.IsActive,
            WalletId = transactionDto.WalletId,
            CategoryId = transactionDto.CategoryId,
            CreatedAt = transactionDto.Date,
            Description = transactionDto.Description
        };
        wallet.Balance -= transaction.Amount;
        
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Post), new { id = transaction.Id }, transaction);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if(transaction == null) return NotFound();
        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}