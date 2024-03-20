using Ambition.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambition.Infrastructure.Data.Customers;

internal class ProducerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.Property(x => x.Name)
            .IsRequired(true)
            .HasMaxLength(50);

        builder.Property(x => x.Address)
            .IsRequired(true)
            .HasMaxLength(1000);

        builder.Property(x => x.Email)
            .IsRequired(true)
            .HasMaxLength(100);

        builder.Property(x => x.Phone)
            .IsRequired(true)
            .HasMaxLength(20);
    }
}
