using Finances.Models.Enums;

namespace Finances.DTOs;

public class TransactionGetDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public bool IsActive { get; set; }
    public Guid WalletId { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryTitle { get; set; }
    public DateTime? Date { get; set; }
}