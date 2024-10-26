using Ambition.Domain;
using Ambition.UI;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureSerilog();

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

