using Finances.Models.Enums;

namespace Finances.Models;

public class TransactionEntity : BaseTitleDescriptionEntity
{
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public bool IsActive { get; set; } = true;
    
    public Guid WalletId { get; set; }
    public WalletEntity Wallet { get; set; }
    
    public Guid CategoryId { get; set; }
    public CategoryEntity Category { get; set; }
}