namespace Gastronomy.Dtos;

public class DishCategoryDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public override string ToString() => Name;
}