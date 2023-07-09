using Microsoft.EntityFrameworkCore;
using Skol.Messaging.Ingress.Domain.Models;

namespace Skol.Messaging.Ingress.Internals.Storage
{
    public interface IIntentsDb
    {
        DbSet<Topic> Topics { get; set; }
        DbSet<Subscription> Subscriptions { get; set; }
    }
}
