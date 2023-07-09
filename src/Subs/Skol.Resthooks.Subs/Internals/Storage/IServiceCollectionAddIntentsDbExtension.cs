using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Skol.Resthooks.Subs.Internals.Storage;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionAddIntentsDbExtension
    {
        public static IServiceCollection AddIntentsDb(this IServiceCollection services)
        {
            services.AddStateChangeHandler()
                    .AddIntentsDbInternal();

            return services;
        }

        private static IServiceCollection AddStateChangeHandler(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()))
                    .AddSingleton<StateChangeHandler>();

            return services;
        }

        private static IServiceCollection AddIntentsDbInternal(this IServiceCollection services)
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
