using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Skol.Messaging.Ingress.Domain;

namespace Skol.Messaging.Ingress.Internals.Http
{
    internal static class IHeaderDictionaryWithoutPoisonedExtension
    {
        private static readonly StringValues PoisonedHeaderNames = new StringValues(new[]
        {
            // HINT: https://github.com/dotnet/aspnetcore/issues/16797
            HeaderNames.Connection,
            // HINT: https://github.com/aspnet/javascriptservices/issues/1469
            HeaderNames.Accept, HeaderNames.Host, HeaderNames.UserAgent, HeaderNames.Upgrade,
            HeaderNames.SecWebSocketKey, HeaderNames.SecWebSocketProtocol, HeaderNames.SecWebSocketVersion, HeaderNames.SecWebSocketAccept,
            // HINT: Content is set when serializing.
            HeaderNames.ContentType, HeaderNames.ContentLength,
            // HINT: .SendAsync removes chunking, so remove the header as well.
            HeaderNames.TransferEncoding,
            // HINT: HTTP/2 or 3 and to HTTP1.1 or less.
            HeaderNames.KeepAlive, HeaderNames.Upgrade, "Proxy-Connection",

            // HINT: Resthooks decided how to pass these.
            HeaderNames.Authorization, HeaderNames.ProxyAuthorization, HeaderNames.WWWAuthenticate, HeaderNames.ProxyAuthenticate,
            HeaderNames.Date,
            KnownHeaderNames.OccurredAsOf
        });

        public static IDictionary<string, string> WithoutPoisoned(this IHeaderDictionary headers)
            => headers.Where(header => !Poisoned(header))
                      .ToDictionary();

        private static bool Poisoned(KeyValuePair<string, StringValues> header)
            => PoisonedHeaderNames.Contains(header.Key, StringComparer.OrdinalIgnoreCase);
    }
}
