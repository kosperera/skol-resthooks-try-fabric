using Skol.Resthooks.Subs.Internals.Http;
using static System.Environment;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionAddMessagingProxyExtension
{
    public static IServiceCollection AddMessagingProxy(this IServiceCollection services)
    {
        services.AddHttpClient<IPublishingEndpoint, MessagingHttpClient>()
                .ConfigureHttpClient((ctor, https) =>
                {
                    //var cfg = ctor.GetRequiredService<IConfiguration>();
                    //var endpoint = cfg.GetValue<Uri>("SKOL_INGRESS_ENDPOINT_URL");
                    https.BaseAddress = new Uri(GetEnvironmentVariable("SKOL_INGRESS_ENDPOINT_URL"));
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));

        return services;
    }
}
