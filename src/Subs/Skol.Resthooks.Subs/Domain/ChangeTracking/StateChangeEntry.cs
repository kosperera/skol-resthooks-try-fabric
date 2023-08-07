using System;

namespace Skol.Resthooks.Subs.Domain.ChangeTracking;

public abstract record StateChangeEntry
{
    public DateTimeOffset OccurredAsOf => DateTimeOffset.UtcNow;
    public bool Published { get; set; } = false;
}
