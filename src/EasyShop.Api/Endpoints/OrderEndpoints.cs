using EasyShop.Application.Orders.Commands.Cancel;
using EasyShop.Application.Orders.Commands.Confirm;
using EasyShop.Application.Orders.Commands.Create;
using EasyShop.Application.Orders.Queries.Detail;
using MediatR;

namespace EasyShop.Api.Endpoints;

public static class OrderEndpoints
{
    public static void RegisterOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("orders");

        group.MapPost("/", async (IMediator mediator, CreateOrderRequest request) =>
        {
            var response = await mediator.Send(request);
            if (response.IsError)
                return Results.BadRequest(response.Errors);

            return Results.Ok(response.Value);
        });

        group.MapPost("/{id:guid}/cancel", async (IMediator mediator, Guid id, CancellationToken cancellationToken) =>
        {
            var request = new CancelOrderRequest
            {
                Id = id
            };
            var response = await mediator.Send(request, cancellationToken);
            if (response.IsError)
                return Results.BadRequest(response.Errors);

            return Results.Ok(response.Value);
        });

        group.MapPost("/{id:guid}/confirm", async (IMediator mediator, Guid id, CancellationToken cancellationToken) =>
        {
            var request = new ConfirmOrderRequest
            {
                Id = id,
                WarehouseId = Guid.NewGuid(),
            };

            var response = await mediator.Send(request, cancellationToken);
            if (response.IsError)
                return Results.BadRequest(response.Errors);

            return Results.Ok();
        });

        group.MapGet("/{id:guid}", async (IMediator mediator, Guid id, CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new OrderDetailRequest()
            {
                Id = id
            }, cancellationToken);

            if (response.IsError)
                return Results.BadRequest(response.Errors);

            return Results.Ok(response.Value);
        });
    }
}