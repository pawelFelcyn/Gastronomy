using Gastronomy.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gastronomy.Backend.Database.Configurations;

internal class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.Property(x => x.Name)
               .HasMaxLength(256);
        builder.HasIndex(x => x.Name)
            .IsUnique();
        builder.HasMany(x => x.DishCategories)
            .WithOne(x => x.Restaurant)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(x => x.RestaurantId);
    }
}