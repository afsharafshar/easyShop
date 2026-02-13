using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EasyShop.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("easy-shop", () => HealthCheckResult.Healthy());

        services.AddHttpContextAccessor();
        
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        return services;
    }
}