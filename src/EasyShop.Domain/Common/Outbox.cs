namespace EasyShop.Domain.Common;

public class Outbox : Entity
{
    public string Type { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsDone { get; set; }
    public DateTime CreeatedAt { get; set; } = DateTime.UtcNow;
}