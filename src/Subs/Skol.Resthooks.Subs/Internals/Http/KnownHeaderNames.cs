using Microsoft.Net.Http.Headers;

namespace Skol.Resthooks.Subs.Internals.Http
{
    internal static class KnownHeaderNames
    {
        public static string EventKind => "X-Event-Kind";
        public static string Environment => "X-Environment";
        public static string SubscriptionId => "X-Subscription-Id";
    }
}
