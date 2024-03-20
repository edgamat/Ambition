using Ambition.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambition.Infrastructure.Data.MaintenancePlans;

internal class CustomerConfiguration : IEntityTypeConfiguration<MaintenancePlan>
{
    public void Configure(EntityTypeBuilder<MaintenancePlan> builder)
    {
        builder.ToTable("MaintenancePlans");

        builder.Property(x => x.Cost)
            .HasPrecision(8, 4);

        builder.Property(x => x.CreatedBy)
            .IsRequired(true)
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .IsRequired(true)
            .HasMaxLength(1000);
    }
}
