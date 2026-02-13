using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EasyShop.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("easy-shop", () => HealthCheckResult.Healthy());

        services.AddHttpContextAccessor();
        return services;
    }
}