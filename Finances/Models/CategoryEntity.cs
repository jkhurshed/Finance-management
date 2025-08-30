using System.Text.Json.Serialization;

namespace Finances.Models;

public class CategoryEntity : BaseTitleDescriptionEntity
{
    public string? Icon { get; set; }

    public Guid? ParentCategoryId { get; set; }
    [JsonIgnore]
    public CategoryEntity? ParentCategory { get; set; }
    public ICollection<CategoryEntity> SubCategories { get; set; } = new List<CategoryEntity>();
   
    [JsonIgnore]
    public ICollection<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();
}