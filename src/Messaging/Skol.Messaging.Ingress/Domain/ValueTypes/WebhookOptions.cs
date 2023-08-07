using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Skol.Messaging.Ingress.Domain.ValueTypes;

public sealed record WebhookOptions(
    [property: JsonPropertyName("notification_url")] string NotificationUrl,
    MetadataEntry Version,
    [property: JsonPropertyName("access_token")] MetadataEntry AccessToken,
    [property: JsonPropertyName("signing_key")] string SigningKey);

