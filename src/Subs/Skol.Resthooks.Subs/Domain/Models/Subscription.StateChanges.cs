using System.Collections.Generic;
using System.Text.Json.Serialization;
using Skol.Resthooks.Subs.Domain.ChangeTracking;

namespace Skol.Resthooks.Subs.Domain.Models
{
    public sealed partial class Subscription : IStateChangeEnumerator
    {
        [property: JsonIgnore]
        public IList<StateChangeEntry> StateChanges { get; } = new List<StateChangeEntry>();
    }
}
