namespace Gastronomy.Dtos;

public sealed class CreateDishDto : IUpsertDish
{
    public string? Name { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsNewCategory { get; set; }
    public Guid? ExistingCategoryId { get; set; }
    public string? NewCategoryName { get; set; }
}