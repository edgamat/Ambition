namespace Ambition.Domain;

public class MaintenancePlan
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid CustomerId { get; set; }

    public string Description { get; set; } = null!;

    public DateTime EffectiveOn { get; set; }

    public decimal Cost { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}