using EasyShop.Api.Endpoints;
using EasyShop.Api.Infrastructure;
using EasyShop.Application;
using EasyShop.Infrastructure;
using EasyShop.Infrastructure.Repositories;
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
builder.Services.AddApplicationServices()
    .AddInfrastructure(builder.Configuration)
    .AddApi();

// builder.Services.AddApi();
var app = builder.Build();

var scope = app.Services.CreateScope();
var sp = scope.ServiceProvider;
var seeder = sp.GetRequiredService<DatabaseSeeder>();
await seeder.EnsureDatabaseExistsAsync(CancellationToken.None);

app.UseCorrelationId();

app.UseSwagger();
app.UseSwaggerUI();

app.MapOpenApi();

app.RegisterHealthEndpoint();
app.RegisterOrderEndpoints();

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