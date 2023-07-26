namespace System.Text.Json
{
    public sealed partial class JsonObjectSerializer
    {
        readonly JsonSerializerOptions _options;

        public JsonObjectSerializer(JsonSerializerOptions options)
        {
            _options = options;
        }

        public T Deserialize<T>(string json, JsonSerializerOptions overrideOptions = default)
            => JsonSerializer.Deserialize<T>(json, overrideOptions ?? _options);

        public string Serialize(object value, JsonSerializerOptions overrideOptions = default)
            => JsonSerializer.Serialize(value, overrideOptions ?? _options);
    }
}
