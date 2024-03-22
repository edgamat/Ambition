var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/send", () =>
{
    return Results.Ok();
})
.WithName("Send")
.WithOpenApi();

app.Run();
