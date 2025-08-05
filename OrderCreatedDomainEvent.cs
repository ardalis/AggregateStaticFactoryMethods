using Mediator;

public sealed class OrderCreatedDomainEvent : INotification
{
    public Guid OrderId { get; }
    public string CustomerId { get; }

    public OrderCreatedDomainEvent(Guid orderId, string customerId)
    {
        OrderId = orderId;
        CustomerId = customerId;
    }
}

