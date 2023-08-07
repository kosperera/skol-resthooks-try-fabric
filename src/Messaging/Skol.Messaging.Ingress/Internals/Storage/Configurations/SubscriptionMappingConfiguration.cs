using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Skol.Messaging.Ingress.Domain.Models;
using Skol.Messaging.Ingress.Domain.ValueTypes;

namespace Skol.Messaging.Ingress.Internals.Storage.Configurations;

internal sealed class SubscriptionMappingConfiguration : EntityTypeConfiguration<Subscription>
{
    readonly JsonObjectSerializer _serializer;

    public SubscriptionMappingConfiguration(JsonObjectSerializer serializer)
    {
        _serializer = serializer;
    }

    public override void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.Property(s => s.Topics).Metadata
                                       .SetValueConverter(new ValueConverter<ICollection<string>, string>(
                                           convertToProviderExpression: (topics) => _serializer.Serialize(topics, default),
                                           convertFromProviderExpression: (json) => _serializer.Deserialize<ICollection<string>>(json, default)));

        builder.Property(s => s.Webhook).Metadata
                                        .SetValueConverter(new ValueConverter<WebhookOptions, string>(
                                            convertToProviderExpression: (webhook) => _serializer.Serialize(webhook, default),
                                            convertFromProviderExpression: (json) => _serializer.Deserialize<WebhookOptions>(json, default)));

        builder.HasQueryFilter(s => !s.Archived);
    }
}
