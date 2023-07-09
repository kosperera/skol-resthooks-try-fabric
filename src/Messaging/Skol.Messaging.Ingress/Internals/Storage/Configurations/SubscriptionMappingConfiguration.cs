using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skol.Messaging.Ingress.Domain.Models;
using Skol.Messaging.Ingress.Domain.ValueTypes;

namespace Skol.Messaging.Ingress.Internals.Storage.Configurations
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
            builder.Property(s => s.Topics).HasConversion(
                                               convertToProviderExpression: (topics) => GetJson(topics),
                                               convertFromProviderExpression: (json) => GetObject<ICollection<string>>(json));

            builder.Property(s => s.Webhook).HasConversion(
                                                convertToProviderExpression: (webhook) => GetJson(webhook),
                                                convertFromProviderExpression: (json) => GetObject<WebhookOptions>(json));
        }

        string GetJson(object value)
            => _serializer.Serialize(value);

        T GetObject<T>(string json)
            => _serializer.Deserialize<T>(json);
    }
}
