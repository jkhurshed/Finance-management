namespace Finances.DTOs;

public class CategoryGetDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid? ParentId { get; set; }
    public ICollection<CategoryGetDto> SubCategories { get; set; }
}