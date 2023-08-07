using System;
using System.Linq;
using Skol.Resthooks.Subs.Domain.Models;

namespace Skol.Resthooks.Subs.Internals.Storage;

public static class IQueryableSubscriptionFiltersExtension
{
    public static IQueryable<Subscription> Enabled(this IQueryable<Subscription> query)
        => query.Where(s => s.Enabled);

    public static IQueryable<Subscription> Disabled(this IQueryable<Subscription> query)
        => query.Where(s => !s.Enabled);

    public static IQueryable<Subscription> WithEnvironment(this IQueryable<Subscription> query, string environment)
        => string.IsNullOrWhiteSpace(environment) ? query : query.Where(s => s.Environment == environment);

    public static IQueryable<Subscription> WithId(this IQueryable<Subscription> query, Guid id)
        => query.Where(s => s.Id == id);
}
