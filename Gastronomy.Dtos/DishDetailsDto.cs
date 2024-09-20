namespace Gastronomy.Dtos;

public sealed class DishDetailsDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
    public decimal BasePrice { get; set; }
    public string? Description { get; set; }
    public Guid DishCategoryId { get; set; }
}