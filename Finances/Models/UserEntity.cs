using System.Text.Json.Serialization;

namespace Finances.Models;

public class UserEntity : BaseEntity
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    
    [JsonIgnore]
    public ICollection<WalletEntity> Wallets { get; set; } = new List<WalletEntity>();
}