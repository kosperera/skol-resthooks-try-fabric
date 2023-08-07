using Skol.Messaging.Ingress.Endpoints.Publish;

namespace Microsoft.AspNetCore.Routing;

public static class WebApplicationAddEndpointsExtension
{
    public static WebApplication MapEndpoints(this WebApplication endpoints)
    {
        endpoints.UseHttpsRedirection();
        endpoints.MapPublishEndpoint();

        return endpoints;
    }
}
