using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Skol.Messaging.Ingress.Domain.Models;

namespace Skol.Messaging.Ingress.Internals.Storage;

internal sealed partial class IntentsDbContext : DbContext, IIntentsDb
{
    readonly IEnumerable<EntityTypeConfiguration> _configurations;

    public DbSet<Topic> Topics { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    public IntentsDbContext(DbContextOptions opt, IEnumerable<EntityTypeConfiguration> configs) : base(opt)
    {
        _configurations = configs;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => base.OnModelCreating(modelBuilder.ApplyConfigurations(_configurations.ToArray()));
}
