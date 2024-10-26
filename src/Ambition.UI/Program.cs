using System.Diagnostics;

using Ambition.Domain;
using Ambition.UI;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureOpenTelemetry();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAmbitionDbContext(builder.Configuration, builder.Environment);
builder.Services.AddMessaging(builder.Configuration, builder.Environment);
builder.Services.AddScoped<IMaintenancePlanService, MaintenancePlanService>();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = (context) =>
    {
        var traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
        if (!string.IsNullOrWhiteSpace(traceId))
        {
            context.ProblemDetails.Extensions["traceId"] = traceId;
            context.ProblemDetails.Detail = "An error occurred in our API. Use the trace id when contacting us.";
        }
    };
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseTraceParent();

app.MapMaintenancePlanEndpoints();

app.Run();

