using Ambition.Domain;

using Microsoft.EntityFrameworkCore;

namespace Ambition.Infrastructure.Data;

internal class AmbitionDbContext : DbContext
{
    public AmbitionDbContext(DbContextOptions<AmbitionDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AmbitionDbContext).Assembly);

        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = Guid.NewGuid(),
                Name = "John Doe",
                Address = "123 Main",
                Email = "test@example.com",
                Phone = "123-456-7890"
            });

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Widget",
                Description = "A widget",
                Price = 10.00m
            });

        base.OnModelCreating(modelBuilder);
    }
}
