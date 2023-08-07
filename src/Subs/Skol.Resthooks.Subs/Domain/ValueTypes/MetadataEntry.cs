using System.Text.Json.Serialization;

namespace Skol.Resthooks.Subs.Domain.ValueTypes;

public sealed record MetadataEntry(
    string Value,
    [property: JsonPropertyName("add_as")] string AddAs,
    [property: JsonPropertyName("add_to")] string AddTo = "Header");
