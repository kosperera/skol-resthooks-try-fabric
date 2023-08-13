using System.Diagnostics;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Skol.Messaging.Contracts;
using Skol.Messaging.Ingress.Domain;
using Skol.Messaging.Ingress.Domain.Models;
using Skol.Messaging.Ingress.Domain.ValueTypes;
using Skol.Messaging.Ingress.Internals.Http;
using Skol.Messaging.Ingress.Internals.Storage;

namespace Skol.Messaging.Ingress.Listeners;

public sealed partial class MessageReceiver : IConsumer<MessageIngested>
{
    readonly IIntentsDb _db;
    readonly ILogger<MessageReceiver> _logger;

    public MessageReceiver(IIntentsDb db, ILogger<MessageReceiver> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<MessageIngested> context)
    {
        MessageIngested msg = context.Message;

        msg.Headers.TryGetValue(KnownHeaderNames.RequestId, out string? requestId);

        // HINT: Assert EventKind is an actual Topic.
        Topic? topic = await _db.Topics.WithName(msg.EventKind)
                                       .AsNoTracking()
                                       .SingleOrDefaultAsync();

        Trace.Assert(
            condition: topic is { },
            message: $"[Resthooks] Ingress notification digest attempt failed. (RequestId={requestId}, MessageId={context.MessageId}, EventKind={msg.EventKind})");

        Subscription[] subscriptions = await _db.Subscriptions
                                                .WithTopic(msg.EventKind)
                                                .WithEnvironment(msg.Environment)
                                                .WithActiveOrBoth(either: topic.IsSystemKind)
                                                // HINT: Default is to query all subscriptions.
                                                //       But scope it to specifics subscription if `X-Subscription-Id` is provided.
                                                .WithIdOrNot(msg.Headers.TryGetValue(KnownHeaderNames.SubscriptionId, out Guid? id) ? id.Value : (Guid?)null)
                                                .ToArrayAsync();

        Trace.Assert(
            condition: subscriptions is { } && subscriptions.Length > 0,
            message: $"[Resthooks] Ingress notification digest attempt failed. (RequestId={requestId}, MessageId={context.MessageId}, EventKind={msg.EventKind}, Environment={msg.Environment})");

        MessageDigested[] messages = subscriptions
            // HINT: Old-school mapping is enuf for now.
            .Select(sub => new MessageDigested(
                Headers: msg.Headers.AddMetadata(new MetadataEntry(Value: msg.EventKind, AddAs: KnownHeaderNames.EventKind))
                                     .AddMetadata(new MetadataEntry(Value: msg.Environment, AddAs: KnownHeaderNames.Environment))
                                     .AddMetadata(new MetadataEntry(Value: $"{sub.Id}", AddAs: KnownHeaderNames.SubscriptionId))                                     .AddMetadata(sub.Webhook.Version)
                                     .AddMetadata(sub.Webhook.AccessToken),
                msg.EventKind,
                msg.Environment,
                msg.OccurredAsOf,

                sub.Webhook.NotificationUrl,
                msg.Content,
                ContentSignature: sub.Webhook.SigningKey))

            .ToArray();

        await context.PublishBatch<MessageDigested>(
            messages,
            callback: (PublishContext<MessageDigested> ctx) =>
            {
                ctx.CorrelationId = context.MessageId;
                ctx.SetRoutingKey(context.RoutingKey());
            });
    }
}
