using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skol.Resthooks.Subs.Domain.Models;
using Skol.Resthooks.Subs.Domain.ValueTypes;

namespace Skol.Resthooks.Subs.Internals.Storage.Configurations
{
    internal sealed class SubscriptionMappingConfiguration : EntityTypeConfiguration<Subscription>
    {
        readonly JsonObjectSerializer _serializer;

        public SubscriptionMappingConfiguration(JsonObjectSerializer serializer)
        {
            _serializer = serializer;
        }

        public override void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.Property(s => s.Id).IsRequired()
                                       .ValueGeneratedNever();

            builder.Property(s => s.DisplayName).IsRequired();
            builder.Property(s => s.Environment).IsRequired();

            builder.Property(s => s.Topics).IsRequired()
                                           .HasConversion(
                                               convertToProviderExpression: (topics) => GetJson(topics),
                                               convertFromProviderExpression: (json) => GetObject<ICollection<string>>(json))

                                           .Metadata
                                           .SetValueComparer(new ValueComparer<ICollection<string>>(
                                               (first, second) => first.SequenceEqual(second),
                                               topics => topics.Aggregate(0, (accum, topic) => HashCode.Combine(accum, topic.GetHashCode())),
                                               topics => (ICollection<string>)topics.ToList()));

            builder.Property(s => s.Webhook).HasConversion(
                                                convertToProviderExpression: (webhook) => GetJson(webhook),
                                                convertFromProviderExpression: (json) => GetObject<WebhookOptions>(json))

                                            .Metadata
                                            .SetValueComparer(new ValueComparer<WebhookOptions>(
                                                (first, second) => first == second,
                                                wh => wh.GetHashCode(),
                                                wh => (WebhookOptions)wh));

            builder.Property(s => s.Enabled).IsRequired()
                                            .HasDefaultValue(false);

            builder.Property(s => s.Archived).IsRequired()
                                             .HasDefaultValue(false);

            builder.Ignore(s => s.StateChanges);
        }

        string GetJson(object value)
            => _serializer.Serialize(value);

        T GetObject<T>(string json)
            => _serializer.Deserialize<T>(json);
    }
}
