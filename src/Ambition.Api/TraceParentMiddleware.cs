using System.Diagnostics;

namespace Ambition.Api;

public class TraceParentMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        if (!context.Response.HasStarted)
        {
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            context.Response.Headers.TryAdd("traceparent", traceId);
        }

        await next.Invoke(context);
    }
}

public static class TraceParentExtensions
{
    public static IApplicationBuilder UseTraceParent(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        return app.UseMiddleware<TraceParentMiddleware>();
    }
}
