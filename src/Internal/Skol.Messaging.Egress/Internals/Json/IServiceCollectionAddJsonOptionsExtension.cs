using System.Text.Json;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionAddJsonOptionsExtension
{
    public static IServiceCollection AddJsonOptions(this IServiceCollection services)
    {
        services.AddOptions<JsonSerializerOptions>()
                .Configure((JsonSerializerOptions opt) => JsonObjectSerializer.Defaults.JsonWebScenario(opt)).Services
                .TryAddSingleton<JsonSerializerOptions>((IServiceProvider ctor) => ctor.GetRequiredService<IOptions<JsonSerializerOptions>>().Value);

        services.TryAddSingleton<JsonObjectSerializer>();

        return services;
    }
}
