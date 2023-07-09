using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Skol.Resthooks.Subs.Domain.ValueTypes
{
    public sealed class WebhookOptions : IEquatable<WebhookOptions>
    {
        [property: JsonPropertyName("notification_url")]
        public string NotificationUrl { get; set; }

        public MetadataEntry Version { get; set; }

        [property: JsonPropertyName("access_token")]
        public MetadataEntry AccessToken { get; set; }

        [property: JsonPropertyName("signing_key")]
        public string SigningKey { get; set; }

        public override bool Equals(object obj)
            => Equals(obj as WebhookOptions);

        public bool Equals(WebhookOptions other)
            => other is { } && (NotificationUrl, Version, AccessToken, SigningKey) == (other.NotificationUrl, other.Version, other.AccessToken, other.SigningKey);

        public override int GetHashCode()
            => HashCode.Combine(NotificationUrl, Version, AccessToken, SigningKey);

        public static bool operator ==(WebhookOptions left, WebhookOptions right)
            => EqualityComparer<WebhookOptions>.Default.Equals(left, right);

        public static bool operator !=(WebhookOptions left, WebhookOptions right)
            => !(left == right);
    }
}
