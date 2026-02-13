using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace EasyShop.Api.Endpoints;

public static class HealthEndpointExtensions
{
    public static void RegisterHealthEndpoint(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions()
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var result = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString()
                    })
                };

                await context.Response.WriteAsJsonAsync(result);
            }
        });
    }
}