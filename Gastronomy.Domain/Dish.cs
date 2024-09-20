namespace Gastronomy.Domain;

public class Dish
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
    public decimal BasePrice { get; set; }
    public string? Description { get; set; }
    public long RowVersion { get; set; }

    public Guid DishCategoryId { get; set; }
    public DishCategory? DishCategory { get; set; }
}