using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skol.Resthooks.Subs.Domain.Models;
using Skol.Resthooks.Subs.Internals.Storage;

namespace Skol.Resthooks.Subs.Endpoints.Subscriptions;

internal static class SubscriptionEndpoint
{
    public static IEndpointRouteBuilder MapSubscriptionEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/v1/subscription/id/{id}", ExecuteAsync)
              .WithName(nameof(SubscriptionEndpoint));

        return routes;
    }

    static async Task<Results<Ok<Subscription>, NotFound<object>>> ExecuteAsync(
        [FromRoute] Guid id,
        IIntentsDb db,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(db);
        ArgumentNullException.ThrowIfNull(logger);

        Subscription? entity = await db.Subscriptions.WithId(id)
                                                     .SingleOrDefaultAsync(cancellationToken);

        return entity is null
            ? TypedResults.NotFound<object>(new { id })
            : TypedResults.Ok<Subscription>(entity);
    }

    public static class Link
    {
        public static string Generate(HttpContext ctx, object command)
            => Generate(ctx.RequestServices.GetRequiredService<LinkGenerator>(), ctx, command);

        static string Generate(LinkGenerator link, HttpContext ctx, object command)
            => link.GetUriByName(ctx, nameof(SubscriptionEndpoint), command);
    }
}
