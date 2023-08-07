using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionAddJsonOptionsExtension
{
    public static IServiceCollection AddJsonOptions(this IServiceCollection services)
    {
        services
            .AddOptions<JsonOptions>()
            .Configure(opt => JsonObjectSerializer.Defaults.JsonWebScenario(opt.JsonSerializerOptions)).Services

            .AddSingleton<JsonObjectSerializer>(ctor =>
            {
                IOptions<JsonOptions> options = ctor.GetRequiredService<IOptions<JsonOptions>>();

                return new JsonObjectSerializer(options.Value.JsonSerializerOptions);
            });

        return services;
    }
}
