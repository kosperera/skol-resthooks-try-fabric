namespace System.Text.Json
{
    public sealed partial class JsonObjectSerializer
    {
        readonly JsonSerializerOptions _options;
        public JsonObjectSerializer(JsonSerializerOptions options)
        {
            _options = options;
        }

        public T Deserialize<T>(string json)
            => JsonSerializer.Deserialize<T>(json, _options);

        public string Serialize(object value)
            => JsonSerializer.Serialize(value, _options);
    }
}
