namespace Ambition.Accounting.Customers
{
    public class CustomerEvent
    {
        public Guid CustomerId { get; set; }

        public long EventId { get; set; }

        public string Description { get; set; } = null!;
    }
}
