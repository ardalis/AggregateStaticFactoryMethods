public sealed class Order : AggregateRoot
{
    public Guid Id { get; private set; }
    public string CustomerId { get; private set; }

    private Order() { } // EF Core

    public static Order Create(string customerId)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId
        };

        order.AddDomainEvent(new OrderCreatedDomainEvent(order.Id, customerId));
        return order;
    }
}

