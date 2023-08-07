using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skol.Resthooks.Subs.Domain.Models;
using Skol.Resthooks.Subs.Internals.Storage;

namespace Skol.Resthooks.Subs.Endpoints.Subscriptions;

internal static class UnsubscribeEndpoint
{
    public static IEndpointRouteBuilder MapUnsubscribeEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapDelete("/v1/subscription/id/{id}", ExecuteAsync)
              .WithName(nameof(UnsubscribeEndpoint));

        return routes;
    }

    static async Task<Results<NoContent, NotFound<object>>> ExecuteAsync([FromRoute] Guid id, IIntentsDb db, CancellationToken cancellationToken)
    {
        Subscription? entity = await db.Subscriptions.WithId(id)
                                                     .AsTracking()
                                                     .SingleOrDefaultAsync(cancellationToken);

        if (entity is null) { return TypedResults.NotFound<object>(new { id }); }

        entity.Archived = true;

        db.Subscriptions.Update(entity);
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }

    public static class Link
    {
        public static string Generate(HttpContext ctx, object command)
            => Generate(ctx.RequestServices.GetRequiredService<LinkGenerator>(), ctx, command);

        static string Generate(LinkGenerator link, HttpContext ctx, object command)
            => link.GetUriByName(ctx, nameof(UnsubscribeEndpoint), command);
    }
}
