using Ambition.Accounting.Customers;

using Microsoft.EntityFrameworkCore;

namespace Ambition.Accounting.Data;

public class AccountingDbContext : DbContext
{
    public AccountingDbContext(DbContextOptions<AccountingDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountingDbContext).Assembly);

        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = Guid.NewGuid(),
                Name = "John Doe",
                Email = "test@example.com"
            });

        base.OnModelCreating(modelBuilder);
    }
}
