using System.Text.Json.Serialization;
using Skol.Messaging.Ingress.Domain.ValueTypes;

namespace Skol.Messaging.Ingress.Domain.Models;

public sealed record Subscription(
    Guid Id,
    string Environment,
    ICollection<string> Topics,
    WebhookOptions Webhook,
    bool Enabled = false,
    [property: JsonIgnore] bool Archived = false);
