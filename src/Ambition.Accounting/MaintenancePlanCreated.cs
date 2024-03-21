namespace Ambition.Domain;

public class MaintenancePlanCreated
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid CustomerId { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; }

    public MaintenancePlanCreated(Guid id, Guid productId, Guid customerId, string createdBy, DateTime createdAt)
    {
        Id = id;
        ProductId = productId;
        CustomerId = customerId;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }
}
