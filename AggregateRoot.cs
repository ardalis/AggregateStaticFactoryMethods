using Mediator;
// see other examples: https://github.com/ardalis/Ardalis.SharedKernel
public abstract class AggregateRoot
{
    private readonly List<INotification> _domainEvents = new();
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(INotification domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}

