using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Skol.Resthooks.Subs.Domain.ValueTypes;

namespace Skol.Resthooks.Subs.Domain.Models
{

    public sealed partial class Subscription
    {
        public Guid Id { get; set; }

        [property: JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
        public string Environment { get; set; }
        public ICollection<string> Topics { get; set; }
        public WebhookOptions Webhook { get; set; }

        [property: JsonPropertyName("activation_code")]
        public string ActivationCode { get; set; }

        public bool Enabled { get; set; } = false;

        [property: JsonIgnore]
        public bool Archived { get; set; } = false;
    }
}
