using Finances.Models.Enums;

namespace Finances.DTOs;

public class WalletCreateDto
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public AccountType AccountType { get; set; }
    public decimal Balance { get; set; }
    public CurrencyVariations Currency { get; set; }
    public Guid UserId { get; set; }
}