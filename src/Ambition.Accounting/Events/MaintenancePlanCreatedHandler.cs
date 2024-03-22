using Ambition.Accounting.Data;
using Ambition.Accounting.Invoices;

using Microsoft.EntityFrameworkCore;

namespace Ambition.Accounting.Events;

public class MaintenancePlanCreatedHandler : IEventHandler<MaintenancePlanCreatedEvent>
{
    private readonly ILogger<MaintenancePlanCreatedHandler> _logger;
    private readonly AccountingDbContext _dbContext;

    public MaintenancePlanCreatedHandler(
        ILogger<MaintenancePlanCreatedHandler> logger,
        AccountingDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task HandleAsync(MaintenancePlanCreatedEvent @event)
    {
        _logger.LogInformation("Handling MaintenancePlanCreated event for maintenance plan: {Id}", @event.Id);

        var invoice = await _dbContext.Set<Invoice>().FirstOrDefaultAsync(i => i.MaintenancePlanId == @event.Id);
        if (invoice != null)
        {
            _logger.LogInformation("Invoice already exists for maintenance plan: {Id}", @event.Id);
            return;
        }

        var invoiceId = Guid.NewGuid();
        invoice = new Invoice
        {
            Id = invoiceId,
            Number = "INV-" + invoiceId.ToString()[..8],
            Amount = 100m,
            InvoicedOn = DateTime.Today,
            DueOn = @event.CreatedAt.Date.AddDays(30),
            MaintenancePlanId = @event.Id,
            CustomerId = @event.CustomerId
        };

        _dbContext.Set<Invoice>().Add(invoice);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Invoice {InvoiceID} created for maintenance plan: {Id}", invoiceId, @event.Id);
    }
}
