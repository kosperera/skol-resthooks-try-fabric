namespace Skol.Messaging.Contracts;

public sealed record MessageDigested(
    IDictionary<string, string> Headers,
    string EventKind,
    string Environment,
    DateTimeOffset OccurredAsOf,
    string NotificationUrl,
    object Content,
    string ContentSignature);
