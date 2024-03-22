using Ambition.Domain;
using Ambition.UI;

using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureOpenTelemetry();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAmbitionDbContext(builder.Configuration, builder.Environment);
builder.Services.AddMessaging(builder.Configuration, builder.Environment);
builder.Services.AddScoped<IMaintenancePlanService, MaintenancePlanService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/maintenance-plan/{id:guid}", async ([FromRoute] Guid id, IMaintenancePlanRepository repository) =>
{
    var plan = await repository.GetByIdAsync(id);

    return plan is null
        ? Results.NotFound()
        : Results.Ok(plan);
})
.WithName("Get")
.WithOpenApi();

app.MapPost("/maintenance-plan", async ([FromBody] CreatePlanModel model, IMaintenancePlanService maintenancePlanService) =>
{
    var plan = new MaintenancePlan
    {
        Id = Guid.NewGuid(),
        Description = model.Description,
        CustomerId = model.CustomerId,
        ProductId = model.ProductId,
        EffectiveOn = model.EffectiveOn,
        CreatedBy = model.UserName,
        CreatedAt = DateTime.UtcNow
    };

    await maintenancePlanService.CreateAsync(plan);

    return Results.Created($"/maintenance-plan/{plan.Id}", model);
})
.WithName("Create")
.WithOpenApi();

app.Run();

