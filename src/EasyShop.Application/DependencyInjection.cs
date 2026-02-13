using System.Reflection;
using EasyShop.Application.Common.Behaviours;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace EasyShop.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // var applicationMarker = typeof(DependencyInjection);
        
        // services.AddValidatorsFromAssembly(applicationMarker.GetTypeInfo().Assembly);

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenRequestPreProcessor(typeof(LoggingBehaviour<>));
            // cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        });
        
        // services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));

        services.AddMapster();
        
        return services;
    }
}