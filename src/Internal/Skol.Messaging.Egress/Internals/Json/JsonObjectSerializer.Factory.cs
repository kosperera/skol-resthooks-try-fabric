namespace System.Text.Json
{
    public sealed partial class JsonObjectSerializer
    {
        internal static class Factory
        {
            public static JsonSerializerOptions GetOrAddDefaults(JsonSerializerOptions options = default)
            {
                options ??= new JsonSerializerOptions();
                options.AllowTrailingCommas = true;
                options.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                options.IgnoreNullValues = true;
                options.PropertyNameCaseInsensitive = true;
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

                return options;
            }
        }
    }
}
