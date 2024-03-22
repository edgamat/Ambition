namespace Ambition.Accounting.Invoices;

public class Invoice
{
    public Guid Id { get; set; }

    public Guid MaintenancePlanId { get; set; }

    public Guid CustomerId { get; set; }

    public string Number { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime InvoicedOn { get; set; }

    public DateTime DueOn { get; set; }
}
