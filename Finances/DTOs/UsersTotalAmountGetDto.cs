namespace Finances.DTOs;

public class UsersTotalAmountGetDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public decimal TotalAmount { get; set; }
}