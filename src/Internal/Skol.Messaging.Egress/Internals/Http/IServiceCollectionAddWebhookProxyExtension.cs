using Skol.Messaging.Egress.Internals.Http;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionAddWebhookProxyExtension
{
    public static IServiceCollection AddWebhookProxy(this IServiceCollection services)
    {
        services.AddHttpClient<WebhookHttpClient>()
                .ConfigureHttpClient((ctor, https) => { })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));

        return services;
    }
}
