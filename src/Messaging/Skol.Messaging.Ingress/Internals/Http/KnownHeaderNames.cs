using Microsoft.Net.Http.Headers;

namespace Skol.Messaging.Ingress.Internals.Http;

internal static class KnownHeaderNames
{
    public static string RequestId => HeaderNames.RequestId;
    public static string OccurredAsOf => HeaderNames.Date;
    public static string EventKind => "X-Event-Kind";
    public static string Environment => "X-Environment";
    public static string SubscriptionId => "X-Subscription-Id";
}
