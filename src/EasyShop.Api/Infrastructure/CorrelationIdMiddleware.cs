namespace EasyShop.Api.Infrastructure;

using System;

public static class CorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorrelationMiddleware>();
    }
}

public sealed class CorrelationMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeaderKey = "X-Correlation-ID";

    public CorrelationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string correlationId;

        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderKey, out var correlationIdValues)
            && correlationIdValues.Count > 0)
        {
            correlationId = correlationIdValues.FirstOrDefault()!;
        }
        else
        {
            correlationId = Guid.NewGuid().ToString();

            context.Request.Headers[CorrelationIdHeaderKey] = correlationId;
        }

        context.Items[CorrelationIdHeaderKey] = correlationId;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeaderKey] = correlationId;
            return Task.CompletedTask;
        });

        await _next(context);
    }
}