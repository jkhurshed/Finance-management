using Finances.DTOs;
using Finances.Models;
using Finances.Models.Enums;
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
        new TransactionGetDto
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
    
    /// <summary>
    /// Get all existing transactions
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<TransactionGetDto>> Get()
    {
        var transactions = await _context.Transactions.ToListAsync();
        var categories = await _context.Categories
            .ToDictionaryAsync(x => x.Id, x => x.Title);
        
        return transactions.Select(t =>
        {
            var categoryTitle = t.CategoryId.HasValue
                ? categories.GetValueOrDefault(t.CategoryId.Value, "Uncategorized")
                : "Uncategorized";

            return TransactionToDto(t, categoryTitle);
        }).ToList();
    }

    /// <summary>
    /// Provide transaction id to see detail info about this transaction
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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
        
        var categoryTitle = transaction.CategoryId.HasValue
            ? categories.GetValueOrDefault(transaction.CategoryId.Value, "Uncategorized")
            : "Uncategorized";
        
        return TransactionToDto(transaction, categoryTitle);
    }

    /// <summary>
    /// Provide transaction id to update the transaction.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Create transaction
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TransactionEntity>> Post(TransactionCreateDto transactionDto)
    {
        var wallet = await _context.Wallets.FindAsync(transactionDto.WalletId);
        if (wallet == null) return NotFound();
        
        if (transactionDto.Amount <= 0)
        {
            return BadRequest("You can't create transaction with amount less then 0!");
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
        
        //Decreasing an amount of money if a transaction type is "Expense"
        if (transactionDto.Type == TransactionType.Expense)
        {
            wallet.Balance -= transaction.Amount;
        }

        //Increasing the amount of money if a transaction type is "Income" 
        if (transactionDto.Type == TransactionType.Income)
        {
            wallet.Balance += transaction.Amount;
        }

        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Post), new { id = transaction.Id }, transaction);
    }
    
    /// <summary>
    /// Deleting a transaction by its id
    /// </summary>
    /// <param name="id"></param>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if(transaction == null) return NotFound();
        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// get transactions by user and month
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("statistics")]
    public async Task<ActionResult<TransactionStatisticsResponseDto>> GetStatistics([FromQuery]TransactionStatisticsRequestDto requestDto)
    {
        var result = await _context.Transactions
            .Where(t => t.Wallet.UserId == requestDto.UserID &&
                        t.CreatedAt.Month == requestDto.Month &&
                        t.CreatedAt.Year == requestDto.Year &&
                        t.TransactionType == TransactionType.Expense)
            .GroupBy(t => new { t.Category.Id, t.Category.Title })
            .Select(g => new TransactionStatisticsResponseDtoDetail
            {
                CategoryID = g.Key.Id,
                CategoryName = g.Key.Title,
                CategoryAmount = g.Sum(t => t.Amount)
            })
            .ToListAsync();

        var response = new TransactionStatisticsResponseDto()
        {
            TotalAmount = result.Sum(r => r.CategoryAmount),
            Details = result,
            Year = requestDto.Year,
            Month = requestDto.Month,
            UserID = requestDto.UserID
        };
        
        return Ok(response);
    }
}