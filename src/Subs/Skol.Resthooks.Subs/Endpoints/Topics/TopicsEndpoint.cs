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

    static async Task<Results<Ok<Topic[]>, NotFound>> ExecuteAsync(IIntentsDb db, ILogger logger, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(db);
        ArgumentNullException.ThrowIfNull(logger);

        Topic[] result = await db.Topics.ToArrayAsync(cancellationToken);

        Log.TopicsFound(logger, result.Length);

        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok<Topic[]>(result);
    }

    private static class Log
    {
        public static void TopicsFound(ILogger logger, int count)
            => logger.LogInformation("[Resthooks] Topics {Count} found.", count);
    }
}
