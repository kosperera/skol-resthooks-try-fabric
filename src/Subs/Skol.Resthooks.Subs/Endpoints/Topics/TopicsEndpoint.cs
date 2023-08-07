using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Skol.Resthooks.Subs.Domain.Models;
using Skol.Resthooks.Subs.Internals.Storage;

namespace Skol.Resthooks.Subs.Endpoints.Topics;

internal static class TopicsEndpoint
{
    public static IEndpointRouteBuilder MapTopicsEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/v1/topics", ExecuteAsync)
              .WithName(nameof(TopicsEndpoint));

        return routes;
    }

    static async Task<Results<Ok<Topic[]>, NotFound>> ExecuteAsync(IIntentsDb db, CancellationToken cancellationToken)
    {
        Topic[] result = await db.Topics.ToArrayAsync(cancellationToken);

        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok<Topic[]>(result);
    }
}
