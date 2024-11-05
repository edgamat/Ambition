using System.Diagnostics;

namespace Ambition.Domain;

internal static class ActivityExtensions
{
    public static void EnrichWithMaintenancePlan(this Activity activity, MaintenancePlan maintenancePlan)
    {
        activity.SetTag(DiagnosticsNames.MaintenancePlanId, maintenancePlan.Id);
        activity.SetTag(DiagnosticsNames.MaintenancePlanProductId, maintenancePlan.ProductId);
        activity.SetTag(DiagnosticsNames.MaintenancePlanCustomerId, maintenancePlan.CustomerId);
        activity.SetTag(DiagnosticsNames.UserName, maintenancePlan.CreatedBy);
    }

    public static Activity? StartRootActivity(this ActivitySource source, string name)
    {
        if (Activity.Current is null)
        {
            return null;
        }

        var rootContext = new ActivityContext(
            ActivityTraceId.CreateRandom(),
            ActivitySpanId.CreateRandom(),
            Activity.Current.ActivityTraceFlags);

        return source.StartActivity(name, ActivityKind.Internal, rootContext);
    }

    public static Activity? StartActivityWithTags(this ActivitySource source,
        string name, List<KeyValuePair<string, object?>> tags)
    {
        return source.StartActivity(name,
            ActivityKind.Internal,
            Activity.Current?.Context ?? new ActivityContext(),
            tags);
    }
}
