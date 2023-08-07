using System.Text.Json.Serialization;
using Skol.Resthooks.Subs.Domain.ChangeTracking;
using static Skol.Resthooks.Subs.Domain.Events.SubscriptionCreated;

namespace Skol.Resthooks.Subs.Domain.Events;

public sealed record SubscriptionCreated(
    MessageMetadata Metadata,
    [property: JsonPropertyName("resource_href")] IDictionary<string, string> ResourceHref)
    : StateChangeEntry
{
    public sealed record MessageMetadata(
        IDictionary<string, string> Headers,
        [property: JsonPropertyName("resource_id")] Guid ResourceId);
};
