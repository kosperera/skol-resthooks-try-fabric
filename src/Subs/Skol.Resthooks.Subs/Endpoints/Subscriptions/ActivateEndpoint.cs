using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skol.Resthooks.Subs.Domain.Models;
using Skol.Resthooks.Subs.Internals.Storage;

namespace Skol.Resthooks.Subs.Endpoints.Subscriptions;

internal static class ActivateEndpoint
{
    public static IEndpointRouteBuilder MapActivateEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapPatch("/v1/subscription/id/{id}/activate", ExecuteAsync)
              .WithName(nameof(ActivateEndpoint));

        return routes;
    }
    static async Task<Results<NoContent, BadRequest, NotFound<object>, UnprocessableEntity<object>>> ExecuteAsync(
        [FromRoute] Guid id,
        [FromBody] ActivateSubscription content,
        IIntentsDb db,
        CancellationToken cancellationToken)
    {
        if (content is null) { return TypedResults.BadRequest(); }

        Subscription? entity = await db.Subscriptions.Disabled()
                                                     .WithId(id)
                                                     .AsTracking()
                                                     .SingleOrDefaultAsync(cancellationToken);

        var routeValues = new { id };

        if (entity is null) { return TypedResults.NotFound<object>(routeValues); }
        else if (entity.ActivationCode != content.Code) { return TypedResults.UnprocessableEntity<object>(routeValues); }

        entity.Enabled = true;

        db.Subscriptions.Update(entity);
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }

    public static class Link
    {
        public static string Generate(HttpContext ctx, object command)
            => Generate(ctx.RequestServices.GetRequiredService<LinkGenerator>(), ctx, command);

        static string Generate(LinkGenerator link, HttpContext ctx, object command)
            => link.GetUriByName(ctx, nameof(ActivateEndpoint), command);
    }

    sealed record ActivateSubscription([property: JsonPropertyName("activation_code")] string Code);
}
