using Microsoft.AspNetCore.Mvc;

namespace Skol.Resthooks.Subs.Endpoints.Subscriptions.Activate
{
    public sealed partial class ActivateEndpoint
    {
        internal static class Link
        {
            public static string Generate(IUrlHelper url, object command)
                => url.Action(
                    action: nameof(ExecuteAsync),
                    controller: nameof(ActivateEndpoint),
                    values: command,
                    protocol: url.ActionContext.HttpContext.Request.Scheme,
                    host: $"{url.ActionContext.HttpContext.Request.Host}");
        }
    }
}
