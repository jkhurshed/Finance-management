namespace Finances.DTOs;

public class CategoryCreateDto
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public Guid? ParentCategoryId { get; set; }
}