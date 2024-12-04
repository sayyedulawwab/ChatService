using ChatService.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Infrastructure.Data;

public sealed class EfDbContext : DbContext, IUnitOfWork
{
    public EfDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EfDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}