namespace PAS.Domain;

public abstract class Entity : Entity<long> {
}

public abstract class Entity<TId> where TId : notnull {
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
