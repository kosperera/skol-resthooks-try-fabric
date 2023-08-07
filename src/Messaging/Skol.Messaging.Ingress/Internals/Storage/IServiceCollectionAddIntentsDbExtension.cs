using Microsoft.EntityFrameworkCore;
using Skol.Messaging.Ingress.Internals.Storage;
using static System.Environment;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionAddIntentsDbExtension
{
    public static IServiceCollection AddIntentsDb(this IServiceCollection services)
    {
        services
            .AddDbContextConfigurations()
            .AddDbContext<IntentsDbContext>((ctor, builder) =>
            {
                builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                       .UseSqlServer(GetEnvironmentVariable("SKOL_CONNECTION_STRINGS_INTENTSDB"))
                       .EnableDetailedErrors()
                       .EnableSensitiveDataLogging();
            },
            contextLifetime: ServiceLifetime.Singleton,
            optionsLifetime: ServiceLifetime.Transient)

            .AddSingleton<IIntentsDb, IntentsDbContext>();

        return services;
    }
}
