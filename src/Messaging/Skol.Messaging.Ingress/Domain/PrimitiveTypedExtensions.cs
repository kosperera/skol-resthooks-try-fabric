using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Primitives;
using Skol.Messaging.Ingress.Domain;

namespace Skol.Messaging.Ingress.Domain
{
    public static class PrimitiveTypedExtensions
    {
        public static string AsString(this StringValues value)
            => value;

        public static IDictionary<string, string> ToDictionary(this IEnumerable<KeyValuePair<string, StringValues>> result)
            => result.ToDictionary(el => el.Key, el => el.Value.AsString());

        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> result)
            => result.ToDictionary(el => el.Key, el => el.Value);

        public static bool TryGetValue(this IDictionary<string, string> values, string key, out Guid? value)
        {
            value = null;

            if (values.TryGetValue(key, out string val) && Guid.TryParse(val, out Guid guid))
            {
                value = guid;
            }

            return value is { };
        }
    }
}
