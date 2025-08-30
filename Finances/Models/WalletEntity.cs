using System.Text.Json.Serialization;
using Finances.Models.Enums;

namespace Finances.Models;

public class WalletEntity : BaseTitleDescriptionEntity
{
    public AccountType AccountType { get; set; }
    public decimal Balance { get; set; }
    public CurrencyVariations Currency { get; set; }
    public bool IsActive { get; set; }
    
    public Guid UserId { get; set; }
    public virtual UserEntity User { get; set; }
    
    [JsonIgnore]
    public ICollection<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();
}