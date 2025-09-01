namespace Finances.Models;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; } = false;
}

public abstract class BaseTitleDescriptionEntity : BaseEntity
{
    public string Title { get; set; }
    public string? Description { get; set; }
}