using System.Diagnostics;

namespace UnitTests.Ambition.Api;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var traceId = ActivityTraceId.CreateRandom();
        var spanId = ActivitySpanId.CreateRandom();

        var context = new ActivityContext(traceId, spanId, ActivityTraceFlags.Recorded);

        var source = new ActivitySource("Ambition.Api");

        var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
        };

        ActivitySource.AddActivityListener(listener);

        using var activity = source.StartActivity("UnitTest1", ActivityKind.Server, context);

        Assert.NotNull(activity);

        Assert.Null(activity.Id);
    }
}
