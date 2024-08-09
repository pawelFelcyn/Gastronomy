using Gastronomy.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gastronomy.Backend.Database.Configurations;

public class DishCategoryConfiguration : IEntityTypeConfiguration<DishCategory>
{
    public void Configure(EntityTypeBuilder<DishCategory> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(50);

        builder.HasIndex(x => new { x.RestaurantId, x.Name })
            .IsUnique();

        builder.HasMany(x => x.Dishes)
            .WithOne(x => x.DishCategory)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(x => x.DishCategoryId);
    }
}