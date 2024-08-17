namespace Gastronomy.Dtos;

public interface IUpsertDish
{
    string? Name { get; set; }
    decimal BasePrice { get; set; }
    bool IsNewCategory { get; set; }
    Guid? ExistingCategoryId { get; set; }
    string? NewCategoryName { get; set; }
}