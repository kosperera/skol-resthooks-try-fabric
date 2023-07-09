using Microsoft.EntityFrameworkCore;
using Skol.Messaging.Ingress.Internals.Storage;
using static Skol.Messaging.Ingress.Internals.Storage.IntentsDbContext;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionAddIntentsDbExtension
    {
        public static IServiceCollection AddIntentsDb(this IServiceCollection services)
        {
            services
                .AddDbContextConfigurations()
                .AddDbContext<IntentsDbContext>((ctor, builder) =>
                {
                    builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                           .UseSqlServer("Server=localhost;Database=Rh_IntentsDb;User Id=sa;Password=StrongPassw0rd;MultipleActiveResultSets=true")
                           .EnableDetailedErrors()
                           .EnableSensitiveDataLogging();
                },
                contextLifetime: ServiceLifetime.Singleton,
                optionsLifetime: ServiceLifetime.Transient)

                .AddSingleton<IIntentsDb, IntentsDbContext>();

            return services;
        }
    }
}
