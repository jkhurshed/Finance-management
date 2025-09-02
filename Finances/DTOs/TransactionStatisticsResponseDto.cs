namespace Finances.DTOs;

public class TransactionStatisticsResponseDto
{
    public decimal TotalAmount { get; set; }
    public Guid UserID { get; set; }
    public List<TransactionStatisticsResponseDtoDetail> Details { get; set; } = new();
    public int Year { get; set; }
    public int Month { get; set; }
}

public class TransactionStatisticsResponseDtoDetail
{
    public Guid CategoryID { get; set; }
    public string CategoryName { get; set; }
    
    public decimal CategoryAmount { get; set; }
}