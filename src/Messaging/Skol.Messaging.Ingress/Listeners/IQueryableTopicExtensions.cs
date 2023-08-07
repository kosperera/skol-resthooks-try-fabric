using System.Linq;
using Skol.Messaging.Ingress.Domain.Models;

namespace Skol.Messaging.Ingress.Listeners;

internal static class IQueryableTopicExtensions
{
    public static IQueryable<Topic> WithName(this IQueryable<Topic> query, string name)
        => query.Where(t => t.Name == name);
}
