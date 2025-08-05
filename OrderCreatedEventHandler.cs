using Mediator;

public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedDomainEvent>
{
    public ValueTask Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Handler] Order created with ID: {notification.OrderId}");
        return ValueTask.CompletedTask;
    }
}
