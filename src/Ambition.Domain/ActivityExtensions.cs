using System.Diagnostics;

namespace Ambition.Domain;

internal static class ActivityExtensions
{
    public static void EnrichWithMaintenancePlan(this Activity activity, MaintenancePlan maintenancePlan)
    {
        activity.SetTag(DiagnosticNames.MaintenancePlanId, maintenancePlan.Id);
        activity.SetTag(DiagnosticNames.MaintenancePlanProductId, maintenancePlan.ProductId);
        activity.SetTag(DiagnosticNames.MaintenancePlanCustomerId, maintenancePlan.CustomerId);
        activity.SetTag(DiagnosticNames.UserName, maintenancePlan.CreatedBy);
    }
}
