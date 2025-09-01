namespace Finances.DTOs;

public class TransactionStatisticsDto
{
    public decimal Total { get; set; }
    public Guid WalletID { get; set; }
    // public Guid CategoryId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}