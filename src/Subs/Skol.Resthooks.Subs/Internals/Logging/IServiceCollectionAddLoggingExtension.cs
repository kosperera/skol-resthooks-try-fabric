using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionAddLoggingExtension
{
    public static IServiceCollection AddLogging<T>(this IServiceCollection services) where T : class
    {
        services.TryAddSingleton<ILogger>((IServiceProvider ctor) => ctor.GetRequiredService<ILoggerFactory>().CreateLogger<T>());

        return services;
    }
}
