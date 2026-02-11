namespace EasyShop.Domain.Common;

public abstract class AggregateRoot : Entity
{
    public byte[] RowVersion { get; protected set; } = default!;
    
    protected AggregateRoot(Guid id) : base(id)
    {
    }

    protected AggregateRoot()
    {
    }

    protected readonly List<IDomainEvent> _domainEvents = new();

    public List<IDomainEvent> PopDomainEvents()
    {
        var copy = _domainEvents.ToList();
        _domainEvents.Clear();

        return copy;
    }
}