using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Skol.Messaging.Ingress.Domain.Models;

namespace Skol.Messaging.Ingress.Listeners;

internal static class IQueryableSubscriptionExtensions
{
    public static IQueryable<Subscription> WithTopic(this DbSet<Subscription> source, string topic)
        => source.FromSqlRaw(@"SELECT * FROM dbo.Subscriptions WHERE @Topic IN (SELECT value FROM OPENJSON(Topics))", new SqlParameter("@Topic", topic));

    public static IQueryable<Subscription> WithEnvironment(this IQueryable<Subscription> query, string environment)
        => query.Where(s => s.Environment == environment);

    public static IQueryable<Subscription> WithActiveOrBoth(this IQueryable<Subscription> query, bool either)
        => either ? query : query.Enabled();

    public static IQueryable<Subscription> Enabled(this IQueryable<Subscription> query)
        => query.Where(s => s.Enabled);

    public static IQueryable<Subscription> WithIdOrNot(this IQueryable<Subscription> query, Guid? id)
        => id is null ? query : query.WithId(id.GetValueOrDefault());

    public static IQueryable<Subscription> WithId(this IQueryable<Subscription> query, Guid id)
        => query.Where(s => s.Id == id);
}
