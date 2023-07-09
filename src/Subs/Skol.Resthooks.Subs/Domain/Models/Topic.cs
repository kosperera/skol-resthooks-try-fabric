using System.Text.Json.Serialization;

namespace Skol.Resthooks.Subs.Domain.Models
{
    public sealed class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [property: JsonPropertyName("is_system_kind")]
        public bool IsSystemKind { get; set; } = false;

        [property: JsonIgnore]
        public bool Archived { get; set; } = false;
    }
}
