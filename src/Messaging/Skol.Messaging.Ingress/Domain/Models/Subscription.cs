using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Skol.Messaging.Ingress.Domain.ValueTypes;

namespace Skol.Messaging.Ingress.Domain.Models
{
    public sealed class Subscription
    {
        public Guid Id { get; set; }
        public string Environment { get; set; }
        public ICollection<string> Topics { get; set; }
        public WebhookOptions Webhook { get; set; }
        public bool Enabled { get; set; } = false;

        [property: JsonIgnore]
        public bool Archived { get; set; } = false;
    }
}
