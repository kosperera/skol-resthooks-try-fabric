using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Skol.Resthooks.Subs.Domain;
using Skol.Resthooks.Subs.Domain.Events;
using Skol.Resthooks.Subs.Domain.Models;
using Skol.Resthooks.Subs.Domain.ValueTypes;
using Skol.Resthooks.Subs.Internals.Storage;
using static System.Environment;

namespace Skol.Resthooks.Subs.Endpoints.Subscriptions;

internal static class SubscribeEndpoint
{
    public static IEndpointRouteBuilder MapSubscribeEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/v1/subscription", ExecuteAsync)
              .WithName(nameof(SubscribeEndpoint));

        return routes;
    }

    static async Task<Results<Created, BadRequest>> ExecuteAsync(
            [FromBody] CreateWebhookSubscription payload,
            HttpContext context,
            IIntentsDb db,
            CancellationToken cancellationToken)
    {
        HttpRequest request = context.Request;

        request.Headers.TryGetValue(HeaderNames.RequestId, out var requestId);

        Trace.Assert(
            condition: payload.Topics.Length > 0,
            message: $"[Resthooks] Subscription attempt failed. (RequestId={requestId})");

        if (payload.Topics.Length == 0) { return TypedResults.BadRequest(); }

        Topic[] topics = await db.Topics
                                        // HINT: Including system kinds
                                        .Where(t => t.IsSystemKind || payload.Topics.Contains(t.Name))
                                        .ToArrayAsync(cancellationToken);

        Trace.Assert(
            condition: topics is { } && payload.Topics.Length > 0,
            message: $"[Resthooks] Subscription attempt failed. (RequestId={requestId})");

        var routeValues = new { id = Guid.NewGuid() };
        Subscription entity = new Subscription
        {
            Id = routeValues.id,
            DisplayName = payload.Name,
            // HINT: Default is set to the value in the ASPNETCORE_ENVIRONMENT env variable.
            Environment = payload.Environment ?? GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            Topics = topics.Select(t => t.Name).ToList(),
            Webhook = new(payload.NotificationUrl, payload.Version, payload.AccessToken, SigningKey: $"sig{Guid.NewGuid()}"),
            ActivationCode = $"whv{Guid.NewGuid()}"
        };

        SubscriptionCreated state = new(
            Metadata: new(Headers: request.Headers.WithoutPoisoned(), ResourceId: routeValues.id),
            ResourceHref: new Dictionary<string, string>
            {
                { "get", SubscriptionEndpoint.Link.Generate(context, routeValues) },
                { "activate", ActivateEndpoint.Link.Generate(context, routeValues) },
                { "unsubscribe", UnsubscribeEndpoint.Link.Generate(context, routeValues) }
            });

        entity.StateChanges.Add(state);

        db.Subscriptions.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.Created(state.ResourceHref["get"]);
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
        HeaderNames.Date
    });

    sealed record CreateWebhookSubscription(
        string Name,
        string[] Topics,
        string Environment,
        [property: JsonPropertyName("notification_url")] string NotificationUrl,
        [property: JsonPropertyName("api_version")] MetadataEntry Version,
        [property: JsonPropertyName("api_access_token")] MetadataEntry AccessToken);
}
