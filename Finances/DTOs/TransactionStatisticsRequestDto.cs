using System.ComponentModel.DataAnnotations;

namespace Finances.DTOs;

public class TransactionStatisticsRequestDto
{
    [Required]
    public Guid UserID {get; set; }
    public int Year { get; set; } = DateTime.Now.Year;
    public int Month { get; set; } = DateTime.Now.Month;
}