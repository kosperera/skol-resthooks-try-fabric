using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skol.Resthooks.Subs.Domain;
using Skol.Resthooks.Subs.Domain.ChangeTracking;
using Skol.Resthooks.Subs.Domain.Events;
using Skol.Resthooks.Subs.Domain.Models;
using Skol.Resthooks.Subs.Internals.Http;
using Skol.Resthooks.Subs.Internals.Storage;
using static Skol.Resthooks.Subs.EventHandlers.SubscriptionIntentWorker.ConfirmSubscriptionIntent;

namespace Skol.Resthooks.Subs.EventHandlers
{
    public sealed partial class SubscriptionIntentWorker : INotificationHandler<StateChangeNotification<SubscriptionCreated>>
    {
        internal sealed class ConfirmSubscriptionIntent
        {
            public sealed class MessageMetadata
            {
                [property: JsonPropertyName("resource_id")]
                public Guid ResourceId { get; set; }

                [property: JsonPropertyName("activation_code")]
                public string ActivationCode { get; set; }

                public string Status { get; set; }
            }

            public MessageMetadata Metadata { get; set; }

            [property: JsonPropertyName("resource_href")]
            public IDictionary<string, string> ResourceHref { get; set; }
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

            Subscription subscription = await _db.Subscriptions.WithId(entry.Metadata.ResourceId)
                                                               .SingleOrDefaultAsync(cancellationToken);

            Trace.Assert(
                condition: subscription is { },
                message: $"[Resthooks] Subscriber intent notification attempt failed. ({nameof(Subscription)}={entry.Metadata.ResourceId})");

            ConfirmSubscriptionIntent content = new ConfirmSubscriptionIntent
            {
                Metadata = new MessageMetadata
                {
                    ActivationCode = subscription.ActivationCode,
                    ResourceId = subscription.Id,
                    Status = subscription.Archived ? nameof(subscription.Archived) : subscription.Enabled
                        ? nameof(IQueryableSubscriptionFiltersExtension.Enabled)
                        : nameof(IQueryableSubscriptionFiltersExtension.Disabled)
                },
                ResourceHref = entry.ResourceHref.Where(h => h.Key == "activate").ToDictionary()
            };

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
}
