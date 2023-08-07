using Skol.Resthooks.Subs.Endpoints.Subscriptions;
using Skol.Resthooks.Subs.Endpoints.Topics;

namespace Microsoft.AspNetCore.Routing;

internal static class WebApplicationMapEndpointsExtension
{
    public static WebApplication MapEndpoints(this WebApplication endpoints)
    {
        endpoints.UseHttpsRedirection();

        endpoints.MapTopicsEndpoint()

                 .MapSubscriptionsEndpoint()
                 .MapSubscriptionEndpoint()
                 .MapSubscribeEndpoint()
                 .MapActivateEndpoint()
                 .MapUnsubscribeEndpoint();

        return endpoints;
    }
}
