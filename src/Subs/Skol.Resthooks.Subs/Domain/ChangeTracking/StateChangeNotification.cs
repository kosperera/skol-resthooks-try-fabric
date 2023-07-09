using MediatR;

namespace Skol.Resthooks.Subs.Domain.ChangeTracking
{
    public sealed class StateChangeNotification<TEntry> : INotification where TEntry : notnull, StateChangeEntry
    {
        public TEntry StateChangeEntry { get; }

        public StateChangeNotification(TEntry e)
            => StateChangeEntry = e;
    }
}
