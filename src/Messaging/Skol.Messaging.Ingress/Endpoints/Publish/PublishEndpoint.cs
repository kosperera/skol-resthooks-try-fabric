using System.Diagnostics;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Skol.Messaging.Contracts;
using Skol.Messaging.Ingress.Domain;
using Skol.Messaging.Ingress.Internals.Http;

namespace Skol.Messaging.Ingress.Endpoints.Publish;

internal static class PublishEndpoint
{
    public static IEndpointRouteBuilder MapPublishEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/v1/messaging/publish", ExecuteAsync);

        return routes;
    }

    static async Task<IResult> ExecuteAsync(
        [FromBody] object payload,
        HttpRequest request,
        IPublishEndpoint publisher,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(publisher);
        ArgumentNullException.ThrowIfNull(logger);

        request.Headers.TryGetValue(KnownHeaderNames.RequestId, out var requestId);

        await publisher.Publish<MessageIngested>(new
        {
            OccurredAsOf = request.GetTypedHeaders().Date ?? DateTimeOffset.UtcNow,
            EventKind = request.Headers[KnownHeaderNames.EventKind].AsString(),
            Environment = request.Headers[KnownHeaderNames.Environment].AsString(),

            Headers = request.Headers.WithoutPoisoned(),
            Content = payload,
        },
        callback: (PublishContext<MessageIngested> ctx) =>
        {
            ctx.SetRoutingKey(ctx.Message.EventKind);
        },
        cancellationToken);

        return TypedResults.Accepted(uri: string.Empty);
    }

    static IDictionary<string, string> WithoutPoisoned(this IHeaderDictionary headers)
        => headers.Where(header => !Poisoned(header))
                  .ToDictionary();

    static bool Poisoned(KeyValuePair<string, StringValues> header)
        => PoisonedHeaderNames.Contains(header.Key, StringComparer.OrdinalIgnoreCase);

    static readonly StringValues PoisonedHeaderNames = new StringValues(new[]
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
}
