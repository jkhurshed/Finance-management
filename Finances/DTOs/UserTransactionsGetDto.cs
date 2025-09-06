using Finances.Models.Enums;

namespace Finances.DTOs;

public class UserTransactionsGetDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public string? TransactionTitle { get; set; }
    public string? Description { get; set; }
    public decimal TransactionAmount { get; set; }
    public TransactionType TransactionType { get; set; }
    public string WalletTitle { get; set; }
    public Guid? CategoryId { get; set; }
    public DateTime? Date { get; set; }
}