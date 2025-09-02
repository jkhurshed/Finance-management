using Finances.Models.Enums;

namespace Finances.DTOs;

public class TransactionCreateDto
{
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public bool IsActive { get; set; }
    public Guid WalletId { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Description { get; set; }
}