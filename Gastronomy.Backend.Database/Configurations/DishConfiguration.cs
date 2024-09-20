using Gastronomy.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gastronomy.Backend.Database.Configurations;

public class DishConfiguration : IEntityTypeConfiguration<Dish>
{
    public void Configure(EntityTypeBuilder<Dish> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(50);
        builder.Property(x => x.Description)
            .HasMaxLength(500);
        builder.HasIndex(x => new { x.Name, x.DishCategoryId })
            .IsUnique();
        builder.Property(x => x.BasePrice)
            .HasColumnType("money");

        builder.Property(x => x.RowVersion)
            .IsRowVersion()
            .HasConversion(x => BitConverter.GetBytes(x), x => BitConverter.ToInt64(x))
            .HasColumnType("rowversion");
    }
}