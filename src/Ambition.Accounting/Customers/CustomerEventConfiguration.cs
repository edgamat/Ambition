using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambition.Accounting.Customers
{
    internal class CustomerEventConfiguration : IEntityTypeConfiguration<CustomerEvent>
    {
        public void Configure(EntityTypeBuilder<CustomerEvent> builder)
        {
            builder.ToTable("CustomerEvents");

            builder.HasKey(x => x.EventId);

            builder.Property(x => x.Description)
                .IsRequired(true)
                .HasMaxLength(100);
        }
    }
}