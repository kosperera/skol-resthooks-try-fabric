using System.Text.Json.Serialization;

namespace Skol.Messaging.Ingress.Domain.Models
{
    public sealed class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [property: JsonPropertyName("is_system_kind")]
        public bool IsSystemKind { get; set; } = false;

        public bool Archived { get; set; } = false;
    }
}
