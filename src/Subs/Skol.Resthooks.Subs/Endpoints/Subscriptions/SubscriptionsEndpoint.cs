using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Skol.Resthooks.Subs.Domain.Models;
using Skol.Resthooks.Subs.Internals.Storage;
using static System.Environment;

namespace Skol.Resthooks.Subs.Endpoints.Subscriptions;

internal static class SubscriptionsEndpoint
{
    public static IEndpointRouteBuilder MapSubscriptionsEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/v1/subscriptions", ExecuteAsync)
              .WithName(nameof(SubscriptionsEndpoint));

        return routes;
    }

    static async Task<Results<Ok<Subscription[]>, NotFound>> ExecuteAsync(IIntentsDb db, CancellationToken cancellationToken)
    {
        Subscription[] result = await db.Subscriptions.WithEnvironment(GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
                                                      .ToArrayAsync(cancellationToken);

        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok<Subscription[]>(result);
    }
}
