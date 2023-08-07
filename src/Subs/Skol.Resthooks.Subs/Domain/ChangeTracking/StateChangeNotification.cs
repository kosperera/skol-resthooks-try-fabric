using MediatR;

namespace Skol.Resthooks.Subs.Domain.ChangeTracking;

public sealed record StateChangeNotification<TEntry>(TEntry StateChangeEntry) : INotification
    where TEntry : notnull, StateChangeEntry;
