using System.Text.Json.Serialization;

namespace Skol.Messaging.Ingress.Domain.Models;

public sealed record Topic(
    int Id,
    string Name,
    [property: JsonPropertyName("is_system_kind")] bool IsSystemKind = false,
    bool Archived = false);
