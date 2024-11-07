using Ambition.Api;
using Ambition.Api.MaintenancePlans;
using Ambition.Api.Monitoring;
using Ambition.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureSerilog();
builder.ConfigureHealthChecks();

builder.Services.AddProblemDetailsWithTraceId();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAmbitionDbContext(builder.Configuration, builder.Environment);
builder.Services.AddMessaging();
builder.Services.AddScoped<IMaintenancePlanService, MaintenancePlanService>();

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

