using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionAddJsonOptionsExtension
{
    public static IServiceCollection AddJsonOptions(this IServiceCollection services)
    {
        services.AddOptions<JsonOptions>()
                .Configure(opt => JsonObjectSerializer.Defaults.JsonWebScenario(opt.JsonSerializerOptions)).Services
                .TryAddSingleton<JsonSerializerOptions>((IServiceProvider ctor) => ctor.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions);

        services.TryAddSingleton<JsonObjectSerializer>();

        return services;
    }
}
