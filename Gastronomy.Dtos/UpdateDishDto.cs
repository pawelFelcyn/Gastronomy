namespace Gastronomy.Dtos;

public sealed class UpdateDishDto : IUpsertDish
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public decimal BasePrice { get; set; }
    public string? Description { get; set; }
    public bool IsNewCategory { get; set; }
    public Guid? ExistingCategoryId { get; set; }
    public string? NewCategoryName { get; set; }
}