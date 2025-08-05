using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class DomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _mediator;

    public DomainEventsInterceptor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        // Do nothing before save (events should be published AFTER commit)
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            await DispatchDomainEventsAsync(eventData.Context);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEventsAsync(DbContext context)
    {
        var entitiesWithEvents = context.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
        }

        entitiesWithEvents.ForEach(e => e.ClearDomainEvents());
    }
}
