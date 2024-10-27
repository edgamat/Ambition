using System.Diagnostics;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProblemDetailsWithTraceId(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services.AddProblemDetails(options => options.CustomizeProblemDetails = (context) =>
        {
            var traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
            if (!string.IsNullOrWhiteSpace(traceId))
            {
                context.ProblemDetails.Extensions["traceId"] = traceId;
                context.ProblemDetails.Detail = "An error occurred in our API. Use the trace id when contacting us.";
            }
        });
    }
}
