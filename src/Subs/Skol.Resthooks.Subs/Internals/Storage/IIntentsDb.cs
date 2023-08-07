using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Skol.Resthooks.Subs.Domain.Models;

namespace Skol.Resthooks.Subs.Internals.Storage;

public interface IIntentsDb
{
    public DbSet<Topic> Topics { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
