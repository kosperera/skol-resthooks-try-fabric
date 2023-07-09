using Microsoft.AspNetCore.Mvc;

namespace Skol.Resthooks.Subs.Endpoints.Subscriptions.Get
{
    public sealed partial class SubscriptionEndpoint
    {
        internal static class Link
        {
            public static string Generate(IUrlHelper url, object command)
                => url.Action(
                    action: nameof(ExecuteAsync),
                    controller: nameof(SubscriptionEndpoint),
                    values: command,
                    protocol: url.ActionContext.HttpContext.Request.Scheme,
                    host: $"{url.ActionContext.HttpContext.Request.Host}");
        }
    }
}
