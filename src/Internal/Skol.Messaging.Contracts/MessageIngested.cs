namespace Skol.Messaging.Contracts;

public interface MessageIngested
{
    IDictionary<string, string> Headers { get; set; }
    string EventKind { get; set; }
    string Environment { get; set; }
    DateTimeOffset OccurredAsOf { get; set; }
    object Content { get; set; }
}
