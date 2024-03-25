using System.Diagnostics;

using Ambition.Accounting.Customers;
using Ambition.Accounting.Data;
using Ambition.Accounting.Emails;
using Ambition.Accounting.Invoices;

using Microsoft.EntityFrameworkCore;

namespace Ambition.Accounting.Events;

public class MaintenancePlanCreatedHandler : IEventHandler<MaintenancePlanCreatedEvent>
{
    private readonly ILogger<MaintenancePlanCreatedHandler> _logger;
    private readonly AccountingDbContext _dbContext;
    private readonly IEmailService _emailService;

    public MaintenancePlanCreatedHandler(
        ILogger<MaintenancePlanCreatedHandler> logger,
        AccountingDbContext dbContext,
        IEmailService emailService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _emailService = emailService;
    }

    public async Task HandleAsync(MaintenancePlanCreatedEvent @event)
    {
        _logger.LogInformation("Handling MaintenancePlanCreated event for maintenance plan: {Id}", @event.Id);

        Activity.Current?.SetTag("maintenance-plan.id", @event.Id);
        Activity.Current?.SetTag("costumer_id", @event.CustomerId);
        Activity.Current?.SetTag("product_id", @event.ProductId);
        Activity.Current?.SetTag("user.name", @event.CreatedBy);

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

        Activity.Current?.SetTag("invoice.id", invoiceId);
        Activity.Current?.SetTag("invoice.number", invoice.Number);
        Activity.Current?.SetTag("invoice.customer-id", invoice.CustomerId);

        _dbContext.Set<Invoice>().Add(invoice);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Invoice {InvoiceID} created for maintenance plan: {Id}", invoiceId, @event.Id);

        var customer = await _dbContext.Set<Customer>().FindAsync(@event.CustomerId);
        if (customer == null)
        {
            _logger.LogWarning("Customer {CustomerId} not found for maintenance plan: {Id}", @event.CustomerId, @event.Id);
            return;
        }

        var email = customer.Email;
        var subject = "Invoice for Maintenance Plan";
        var body = $"Dear {customer.Name},\n\nAn invoice has been generated for your maintenance plan. Please find the details below:\n\nInvoice Number: {invoice.Number}\nAmount: {invoice.Amount:C}\nDue Date: {invoice.DueOn:dd-MMM-yyyy}\n\nThank you for choosing our services.\n\nRegards,\nAmbition Accounting Team";

        await _emailService.SendEmailAsync(email, subject, body);
    }
}
