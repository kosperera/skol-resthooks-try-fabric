using System.Text.Json.Serialization;

namespace Skol.Messaging.Ingress.Domain.ValueTypes;

public sealed record MetadataEntry(
    string Value,
    [property: JsonPropertyName("add_as")] string AddAs,
    [property: JsonPropertyName("add_to")] string AddTo = "Header");
