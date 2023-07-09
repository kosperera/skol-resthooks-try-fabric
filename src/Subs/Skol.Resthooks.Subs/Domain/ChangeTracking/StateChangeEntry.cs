using System;

namespace Skol.Resthooks.Subs.Domain.ChangeTracking
{
    public abstract partial class StateChangeEntry
    {
        public DateTimeOffset OccurredAsOf => DateTimeOffset.UtcNow;
        public bool Published { get; set; } = false;
    }
}
