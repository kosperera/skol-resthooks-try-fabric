namespace Skol.Messaging.Egress.Internals.Http;

public static class KnownHeaderNames
{
    public static string CorrelationId => "X-Correlation-Id";
    public static string EventKind => "X-Event-Kind";
    public static string Environment => "X-Environment";
}
