namespace Gastronomy.Domain;

public class DishCategory
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid RestaurantId { get; set; }
    public virtual Restaurant? Restaurant { get; set; }
    public virtual List<Dish>? Dishes { get; set; }
}