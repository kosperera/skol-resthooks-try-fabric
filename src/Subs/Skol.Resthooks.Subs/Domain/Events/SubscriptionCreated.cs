using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Skol.Resthooks.Subs.Domain.ChangeTracking;

namespace Skol.Resthooks.Subs.Domain.Events
{
    public sealed class SubscriptionCreated : StateChangeEntry
    {
        public MessageMetadata Metadata { get; set; }

        [property: JsonPropertyName("resource_href")]
        public IDictionary<string, string> ResourceHref { get; set; }

        public sealed class MessageMetadata
        {
            public IDictionary<string, string> Headers { get; set; }

            [property: JsonPropertyName("resource_id")]
            public Guid ResourceId { get; set; }
        }
    }
}
