﻿namespace Gastronomy.Domain;

public class Restaurant
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public virtual List<DishCategory>? DishCategories { get; set; }
}