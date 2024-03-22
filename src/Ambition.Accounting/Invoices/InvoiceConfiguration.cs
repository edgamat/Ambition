using Ambition.Accounting.Invoices;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambition.Accounting.Customers;

internal class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.Property(x => x.Number)
            .IsRequired(true)
            .HasMaxLength(50);

        builder.Property(x => x.Amount)
            .HasPrecision(8, 4);
    }
}
