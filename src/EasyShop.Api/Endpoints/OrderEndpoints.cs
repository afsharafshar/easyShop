namespace EasyShop.Api.Endpoints;

public static class OrderEndpoints
{
    public static void RegisterOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("order");
        
        group.MapGet("/", () => "Hello World!");
    }
}