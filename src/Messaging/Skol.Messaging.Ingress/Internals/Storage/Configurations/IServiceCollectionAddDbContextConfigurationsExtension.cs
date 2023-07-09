using Microsoft.EntityFrameworkCore;
using Skol.Messaging.Ingress.Internals.Storage.Configurations;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionAddDbContextConfigurationsExtension
    {
        public static IServiceCollection AddDbContextConfigurations(this IServiceCollection services)
        {
            services
                .AddSingleton<EntityTypeConfiguration, SubscriptionMappingConfiguration>()

                .AddSingleton<EntityTypeConfiguration, DefaultSubscriptionsQueryFilter>()
                .AddSingleton<EntityTypeConfiguration, DefaultTopicsQueryFilter>();

            return services;
        }
    }
}
