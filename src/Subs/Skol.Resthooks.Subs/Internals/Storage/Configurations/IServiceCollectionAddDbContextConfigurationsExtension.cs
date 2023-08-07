using Microsoft.EntityFrameworkCore;
using Skol.Resthooks.Subs.Internals.Storage.Configurations;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionAddDbContextConfigurationsExtension
{
    public static IServiceCollection AddDbContextConfigurations(this IServiceCollection services)
    {
        services
            .AddSingleton<EntityTypeConfiguration, SubscriptionMappingConfiguration>()
            .AddSingleton<EntityTypeConfiguration, TopicMappingConfiguration>();

        return services;
    }
}
