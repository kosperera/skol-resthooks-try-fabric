using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Primitives;

namespace Skol.Resthooks.Subs.Domain;

public static class PrimitiveTypedExtensions
{
    public static string AsString(this StringValues value)
        => value;

    public static IDictionary<string, string> ToDictionary(this IEnumerable<KeyValuePair<string, StringValues>> result)
        => result.ToDictionary(el => el.Key, el => el.Value.AsString());

    public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> result)
        => result.ToDictionary(el => el.Key, el => el.Value);
}
