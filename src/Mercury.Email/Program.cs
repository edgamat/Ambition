var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// app.UseHttpsRedirection();

app.MapPost("/send", async (ILogger<Program> logger) =>
{
    logger.LogInformation("Sending email");

    await Task.Delay(TimeSpan.FromSeconds(2));

    logger.LogInformation("Email sent");

    return Results.Ok();
})
.WithName("Send")
.WithOpenApi();

app.Run();
