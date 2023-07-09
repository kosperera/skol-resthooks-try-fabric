using Skol.Messaging.Ingress.Domain.ValueTypes;
using System.Collections.Generic;

namespace Skol.Messaging.Ingress.Listeners
{
    public static class IDictionaryAddMetadataExtensions
    {
        public static IDictionary<string, string> AddMetadata(this IDictionary<string, string> headers, MetadataEntry meta)
        {
            if (headers.ContainsKey(meta.AddAs))
            {
                headers.Remove(meta.AddAs);
            }
            headers.Add(meta.AddAs, meta.Value);

            return headers;
        }
    }
}
