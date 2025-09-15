using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Finances.Models;

public class UserEntity : IdentityUser<Guid>
{
    public string? FullName { get; set; }
    
    [JsonIgnore]
    public ICollection<WalletEntity> Wallets { get; set; } = new List<WalletEntity>();
}