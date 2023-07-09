using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Skol.Resthooks.Subs.Domain.ValueTypes
{
    public sealed class MetadataEntry : IEquatable<MetadataEntry>
    {
        public string Value { get; set; }

        [property: JsonPropertyName("add_to")]
        public string AddTo { get; set; } = "Header";

        [property: JsonPropertyName("add_as")]
        public string AddAs { get; set; }

        public override bool Equals(object obj)
            => Equals(obj as MetadataEntry);

        public bool Equals(MetadataEntry other)
            => other is { } && (Value, AddTo, AddAs) == (other.Value, other.AddTo, AddAs);

        public override int GetHashCode()
            => HashCode.Combine(Value, AddTo, AddAs);

        public static bool operator ==(MetadataEntry left, MetadataEntry right)
            => EqualityComparer<MetadataEntry>.Default.Equals(left, right);

        public static bool operator !=(MetadataEntry left, MetadataEntry right)
            => !(left == right);
    }
}
