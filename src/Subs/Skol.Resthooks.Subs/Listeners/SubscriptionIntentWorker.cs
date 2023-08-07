using System.Diagnostics;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Skol.Resthooks.Subs.Domain;
using Skol.Resthooks.Subs.Domain.ChangeTracking;
using Skol.Resthooks.Subs.Domain.Events;
using Skol.Resthooks.Subs.Domain.Models;
using Skol.Resthooks.Subs.Internals.Http;
using Skol.Resthooks.Subs.Internals.Storage;
using static Skol.Resthooks.Subs.EventHandlers.SubscriptionIntentWorker.ConfirmSubscriptionIntent;

namespace Skol.Resthooks.Subs.EventHandlers;

public sealed partial class SubscriptionIntentWorker : INotificationHandler<StateChangeNotification<SubscriptionCreated>>
{
    internal sealed record ConfirmSubscriptionIntent(
        MessageMetadata Metadata,
        [property: JsonPropertyName("resource_href")] IDictionary<string, string> ResourceHref)
    {
        public sealed record MessageMetadata(
            [property: JsonPropertyName("resource_id")] Guid ResourceId,
            [property: JsonPropertyName("activation_code")] string ActivationCode,
            string Status);
    }

    const string TopicName = "KitchenSync.Intent.Notification";

    readonly IPublishingEndpoint _publisher;
    readonly IIntentsDb _db;
    readonly ILogger<SubscriptionIntentWorker> _logger;

    public SubscriptionIntentWorker(IPublishingEndpoint publisher, IIntentsDb db, ILogger<SubscriptionIntentWorker> logger)
    {
        _publisher = publisher;
        _db = db;
        _logger = logger;
    }

    public async Task Handle(StateChangeNotification<SubscriptionCreated> notification, CancellationToken cancellationToken)
    {
        SubscriptionCreated entry = notification.StateChangeEntry;

        Subscription? subscription = await _db.Subscriptions.WithId(entry.Metadata.ResourceId)
                                                            .SingleOrDefaultAsync(cancellationToken);

        Trace.Assert(
            condition: subscription is { },
            message: $"[Resthooks] Subscriber intent notification attempt failed. ({nameof(Subscription)}={entry.Metadata.ResourceId})");

        ConfirmSubscriptionIntent content = new(
            Metadata: new(
                ActivationCode: subscription.ActivationCode,
                ResourceId: subscription.Id,
                Status: subscription.Archived ? nameof(subscription.Archived) : subscription.Enabled
                    ? nameof(IQueryableSubscriptionFiltersExtension.Enabled)
                    : nameof(IQueryableSubscriptionFiltersExtension.Disabled)),
            ResourceHref: entry.ResourceHref.Where(h => h.Key == "activate").ToDictionary());

        await _publisher.PublishAsync(
            content,
            (req) =>
            {
                foreach (var header in entry.Metadata.Headers)
                {
                    req.Headers.Add(header.Key, header.Value);
                }
                req.Headers.Date = entry.OccurredAsOf;
                req.Headers.Add(KnownHeaderNames.EventKind, TopicName);
                req.Headers.Add(KnownHeaderNames.Environment, subscription.Environment);
                // HINT: Scope is default to X-Environment, X-Event-Kind.
                //       but here we are narrowing it further.
                req.Headers.Add(KnownHeaderNames.SubscriptionId, $"{subscription.Id}");
            },
            cancellationToken);

        notification.StateChangeEntry.Published = true;
    }
}
