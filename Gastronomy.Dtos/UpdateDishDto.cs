namespace Gastronomy.Dtos;

public sealed class UpdateDishDto
{
    public required string Name { get; set; }
    public decimal BasePrice { get; set; }
    public string? Description { get; set; }
    public bool IsNewCategory { get; set; }
    public Guid? ExistingDishCategoryId { get; set; }
    public string? NewCategoryName { get; set; }
}