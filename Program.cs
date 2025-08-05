using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("When to use static factories with DDD entities: domain events edition.");


var services = new ServiceCollection();

services.AddMediator(); // Register Mediator + handlers
services.AddScoped<DomainEventsInterceptor>();

services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.UseInMemoryDatabase("OrdersDb");
    options.AddInterceptors(sp.GetRequiredService<DomainEventsInterceptor>());
});

var provider = services.BuildServiceProvider();

// ---------- Scope 1: Create and Save ----------
using (var scope1 = provider.CreateScope())
{
    var dbContext = scope1.ServiceProvider.GetRequiredService<AppDbContext>();

    // Create an entity via static factory
    var order = Order.Create("Customer123");
    Console.WriteLine($"{nameof(Order)}: {order} instantiated.");

    // Add and save entity (domain event is raised here)
    dbContext.Orders.Add(order);
    Console.WriteLine($"{nameof(Order)}: {order} added to dbContext.");

    Console.WriteLine($"Calling dbContext.SaveChanges...");
    await dbContext.SaveChangesAsync(); // <-- Interceptor publishes the event
    Console.WriteLine($"dbContext.SaveChanges called.");
}

Console.WriteLine("Now a new scope is used to pull the entity from persistence...");

// ---------- Scope 2: Retrieve (no new events) ----------
using (var scope2 = provider.CreateScope())
{
    var dbContext = scope2.ServiceProvider.GetRequiredService<AppDbContext>();

    // Retrieve entity from DB (EF Core materialization does not add events)
    var savedOrder = await dbContext.Orders.FirstAsync();

    // Save again without modifying or adding events
    Console.WriteLine($"Calling dbContext.SaveChanges...");
    await dbContext.SaveChangesAsync(); // <-- Should NOT publish any events
    Console.WriteLine($"dbContext.SaveChanges called.");

}

Console.WriteLine($"Done with scenario 1!");

#region Demo 2
Console.WriteLine();
Console.WriteLine("What if we just use the constructor?");

// ---------- Scope 1: Create and Save ----------
using (var scope1 = provider.CreateScope())
{
    var dbContext = scope1.ServiceProvider.GetRequiredService<AppDbContext>();

    // Create an entity via static factory
    var order = new Order2(Guid.NewGuid(), "Customer234");
    Console.WriteLine($"{nameof(Order2)}: {order} instantiated.");

    // Add and save entity (domain event is raised here)
    dbContext.Order2s.Add(order);
    Console.WriteLine($"{nameof(Order2)}: {order} added to dbContext.");

    Console.WriteLine($"Calling dbContext.SaveChanges...");
    await dbContext.SaveChangesAsync(); // <-- Interceptor publishes the event
    Console.WriteLine($"dbContext.SaveChanges called.");
}

Console.WriteLine("Now a new scope is used to pull the entity from persistence...");

// ---------- Scope 2: Retrieve (no new events) ----------
using (var scope2 = provider.CreateScope())
{
    var dbContext = scope2.ServiceProvider.GetRequiredService<AppDbContext>();

    // Retrieve entity from DB (EF Core materialization does not add events)
    var savedOrder = await dbContext.Order2s.FirstAsync();

    // Save again without modifying or adding events
    Console.WriteLine($"Calling dbContext.SaveChanges...");
    await dbContext.SaveChangesAsync(); // <-- Should NOT publish any events
    Console.WriteLine($"dbContext.SaveChanges called.");
}

Console.WriteLine("Well, how many events were handled?");

#endregion

