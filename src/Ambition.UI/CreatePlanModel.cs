namespace Ambition.UI;

public class CreatePlanModel
{
    public Guid ProductId { get; set; }

    public Guid CustomerId { get; set; }

    public string Description { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public DateTime EffectiveOn { get; set; }
}
