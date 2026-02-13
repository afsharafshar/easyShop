using EasyShop.Api.Endpoints;
using EasyShop.Api.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationIdHeader("X-Correlation-ID")
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();


builder.Services.AddOpenApi();
// builder.Services.AddApplicationServices()
// .AddApi();

builder.Services.AddApi();
var app = builder.Build();

app.UseCorrelationId();

if (app.Environment.IsDevelopment())
{
}

app.MapOpenApi();

app.RegisterHealthEndpoint();

app.UseCors("AllowAll");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}