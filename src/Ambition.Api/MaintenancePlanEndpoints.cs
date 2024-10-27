using Ambition.Api;
using Ambition.Domain;

using Microsoft.AspNetCore.Mvc;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MaintenancePlanEndpoints
{
    public static void MapMaintenancePlanEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/maintenance-plan/{id:guid}", async ([FromRoute] Guid id, IMaintenancePlanRepository repository) =>
        {
            var plan = await repository.GetByIdAsync(id);

            return plan is null
                ? Results.NotFound()
                : Results.Ok(plan);
        })
        .WithName("Get")
        .WithOpenApi();

        endpoints.MapPost("/maintenance-plan", async ([FromBody] CreatePlanModel model, IMaintenancePlanService maintenancePlanService) =>
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

            return Results.Created($"/maintenance-plan/{plan.Id}", plan);
        })
        .WithName("Create")
        .WithOpenApi();
    }
}
