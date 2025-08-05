public sealed class Order2 : AggregateRoot
{
    public Guid Id { get; private set; }
    public string CustomerId { get; private set; }

    public Order2(Guid id, string customerId) 
    {
        Id = id;
        if(Id ==  Guid.Empty) { id = Guid.NewGuid(); }
        CustomerId = customerId;

        AddDomainEvent(new OrderCreatedDomainEvent(Id, CustomerId));
    } 
}

