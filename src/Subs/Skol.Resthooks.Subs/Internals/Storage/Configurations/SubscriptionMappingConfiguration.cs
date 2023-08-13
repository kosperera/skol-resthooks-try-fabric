using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Skol.Resthooks.Subs.Domain.Models;
using Skol.Resthooks.Subs.Domain.ValueTypes;

namespace Skol.Resthooks.Subs.Internals.Storage.Configurations;

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

        builder.Property(s => s.Topics).IsRequired();
        builder.Property(s => s.Topics).Metadata
                                       .SetValueConverter(new ValueConverter<ICollection<string>, string>(
                                           convertToProviderExpression: (topics) => _serializer.Serialize(topics, default),
                                           convertFromProviderExpression: (json) => _serializer.Deserialize<ICollection<string>>(json, default)));
        builder.Property(s => s.Topics).Metadata
                                       .SetValueComparer(new ValueComparer<ICollection<string>>(
                                           (first, second) => first.SequenceEqual(second),
                                           topics => topics.Aggregate(0, (accum, topic) => HashCode.Combine(accum, topic.GetHashCode())),
                                           topics => (ICollection<string>)topics.ToList()));

        builder.Property(s => s.Webhook).Metadata
                                        .SetValueConverter(new ValueConverter<WebhookOptions, string>(
                                            convertToProviderExpression: (webhook) => _serializer.Serialize(webhook, default),
                                            convertFromProviderExpression: (json) => _serializer.Deserialize<WebhookOptions>(json, default)));

        builder.Property(s => s.Enabled).IsRequired()
                                        .HasDefaultValue(false);

        builder.Property(s => s.Archived).IsRequired()
                                         .HasDefaultValue(false);

        builder.Ignore(s => s.StateChanges);

        builder.HasQueryFilter(s => !s.Archived);
    }
}
