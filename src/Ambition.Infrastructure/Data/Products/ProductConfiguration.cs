using Ambition.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambition.Infrastructure.Data.Products;

internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.Property(x => x.Name)
            .IsRequired(true)
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .IsRequired(true)
            .HasMaxLength(1000);

        builder.Property(x => x.Price)
            .HasPrecision(8, 4);
    }
}
