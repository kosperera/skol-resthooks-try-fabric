using MediatR;
using Skol.Resthooks.Subs.Domain.ChangeTracking;

namespace Skol.Resthooks.Subs.Internals.Storage;

internal sealed partial class StateChangeHandler
{
    static Type NotificationShape = typeof(StateChangeNotification<>);

    readonly IPublisher _publisher;
    readonly ILogger _logger;

    public StateChangeHandler(IPublisher publisher, ILogger<StateChangeHandler> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async ValueTask BroadcastAsync(StateChangeEntry[] entries)
    {
        foreach (StateChangeEntry entry in entries)
        {
            await _publisher.Publish(CreateNotification(entry));
        }

        INotification CreateNotification(StateChangeEntry entry)
            => (INotification)Activator.CreateInstance(
                NotificationShape.MakeGenericType(entry.GetType()),
                entry);
    }
}
