using System;
using Skol.Resthooks.Subs.Internals.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionAddMessagingProxyExtension
    {
        public static IServiceCollection AddMessagingProxy(this IServiceCollection services)
        {
            services.AddHttpClient<IPublishingEndpoint, MessagingHttpClient>()
                    .ConfigureHttpClient((ctor, https) => https.BaseAddress = new Uri("https://localhost:8172"))
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

            return services;
        }
    }
}
