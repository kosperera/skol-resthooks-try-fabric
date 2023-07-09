using System.Collections.Generic;

namespace Skol.Resthooks.Subs.Domain.ChangeTracking
{
    public interface IStateChangeEnumerator
    {
        IList<StateChangeEntry> StateChanges { get; }
    }
}
