using Microsoft.EntityFrameworkCore;

public sealed class AppDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Order2> Order2s => Set<Order2>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
