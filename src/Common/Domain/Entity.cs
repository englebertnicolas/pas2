namespace PAS.Domain;

public interface IEntity {
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
}

public abstract class Entity : Entity<long>;

public abstract class Entity<TId> : IEntity where TId : notnull {
    public TId Id { get; protected set; } = default!;

    private readonly List<IDomainEvent> domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

    protected Entity() {
        // For EF hydration
    }

    protected Entity(TId id) {
        Id = id;
    }

    public void AddDomainEvent(IDomainEvent eventItem) {
        domainEvents.Add(eventItem);
    }
}
