using System.Text.Json.Serialization;

namespace Skol.Resthooks.Subs.Domain.ValueTypes;

public sealed record WebhookOptions(
    [property: JsonPropertyName("notification_url")] string NotificationUrl,
    MetadataEntry Version,
    [property: JsonPropertyName("access_token")] MetadataEntry AccessToken,
    [property: JsonPropertyName("signing_key")] string SigningKey);

