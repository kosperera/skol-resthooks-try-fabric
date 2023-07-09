using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Skol.Resthooks.Subs.Domain;
using Skol.Resthooks.Subs.Domain.Events;
using Skol.Resthooks.Subs.Domain.Models;
using Skol.Resthooks.Subs.Domain.ValueTypes;
using Skol.Resthooks.Subs.Endpoints.Subscriptions.Activate;
using Skol.Resthooks.Subs.Endpoints.Subscriptions.Get;
using Skol.Resthooks.Subs.Endpoints.Subscriptions.Unsubscribe;
using Skol.Resthooks.Subs.Internals.Storage;
using static System.Environment;
using static Skol.Resthooks.Subs.Domain.Events.SubscriptionCreated;

namespace Skol.Resthooks.Subs.Endpoints.Subscriptions.Subscribe
{
    [Route("v1/subscription")]
    [ApiController]
    public sealed partial class SubscribeEndpoint : ControllerBase
    {
        public sealed class CreateWebhookSubscription
        {
            public string Name { get; set; }
            public string[] Topics { get; set; }
            public string Environment { get; set; }

            [property: JsonPropertyName("notification_url")]
            public string NotificationUrl { get; set; }

            [property: JsonPropertyName("api_version")]
            public MetadataEntry Version { get; set; }

            [property: JsonPropertyName("api_access_token")]
            public MetadataEntry AccessToken { get; set; }
        }

        readonly IIntentsDb _db;
        readonly ILogger<SubscribeEndpoint> _logger;

        public SubscribeEndpoint(IIntentsDb db, ILogger<SubscribeEndpoint> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpPost]
        public async ValueTask<IActionResult> ExecuteAsync(
            [FromBody] CreateWebhookSubscription payload,
            CancellationToken cancellationToken = default)
        {
            Request.Headers.TryGetValue(HeaderNames.RequestId, out var requestId);

            Trace.Assert(
                condition: payload?.Topics?.Any() == true,
                message: $"[Resthooks] Subscription attempt failed. (RequestId={requestId})");

            if (payload?.Topics?.Any() != true) { return BadRequest(); }

            Topic[] topics = await _db.Topics
                                             // HINT: Including system kinds
                                             .Where(t => t.IsSystemKind || payload.Topics.Contains(t.Name))
                                             .ToArrayAsync(cancellationToken);

            Trace.Assert(
                condition: topics is { } && payload.Topics.Any(),
                message: $"[Resthooks] Subscription attempt failed. (RequestId={requestId})");

            var routeValues = new { id = Guid.NewGuid() };
            Subscription entity = new Subscription
            {
                Id = routeValues.id,
                DisplayName = payload.Name,
                // HINT: Default is set to the value in the ASPNETCORE_ENVIRONMENT env variable.
                Environment = payload.Environment ?? GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                Topics = topics.Select(t => t.Name).ToList(),
                Webhook = new WebhookOptions
                {
                    NotificationUrl = payload.NotificationUrl,
                    Version = payload.Version,
                    AccessToken = payload.AccessToken,
                    SigningKey = $"sig{Guid.NewGuid()}"
                },
                ActivationCode = $"whv{Guid.NewGuid()}"
            };

            SubscriptionCreated state = new SubscriptionCreated
            {
                Metadata = new MessageMetadata
                {
                    Headers = Request.Headers.WithoutPoisoned(),
                    ResourceId = routeValues.id
                },
                ResourceHref = new Dictionary<string, string>
                {
                    { "get", SubscriptionEndpoint.Link.Generate(Url, routeValues) },
                    { "activate", ActivateEndpoint.Link.Generate(Url, routeValues) },
                    { "unsubscribe", UnsubscribeEndpoint.Link.Generate(Url, routeValues) }
                }
            };

            entity.StateChanges.Add(state);

            _db.Subscriptions.Add(entity);
            await _db.SaveChangesAsync(cancellationToken);

            return Created(state.ResourceHref["get"], null);
        }
    }
}
