using Microsoft.EntityFrameworkCore;
using Skol.Resthooks.Subs.Domain.ChangeTracking;
using Skol.Resthooks.Subs.Domain.Models;

namespace Skol.Resthooks.Subs.Internals.Storage;

internal sealed partial class IntentsDbContext : DbContext, IIntentsDb
{
    readonly StateChangeHandler _stateChangeHandler;
    readonly IEnumerable<EntityTypeConfiguration> _configurations;

    public DbSet<Topic> Topics { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    public IntentsDbContext(DbContextOptions opt, IEnumerable<EntityTypeConfiguration> configs, StateChangeHandler stateChangeHandler) : base(opt)
    {
        _configurations = configs;
        _stateChangeHandler = stateChangeHandler;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int entries = await base.SaveChangesAsync(cancellationToken);
        await BroadcastStateChangesAsync();

        return entries;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => base.OnModelCreating(modelBuilder.ApplyConfigurations(_configurations.ToArray()));

    private async ValueTask BroadcastStateChangesAsync()
    {
        StateChangeEntry[] entries = ChangeTracker.Entries<IStateChangeEnumerator>()
                                                  .Select(e => e.Entity.StateChanges)
                                                  .SelectMany(e => e)
                                                  .Where(e => !e.Published)
                                                  .ToArray();

        await _stateChangeHandler.BroadcastAsync(entries);
    }
}
