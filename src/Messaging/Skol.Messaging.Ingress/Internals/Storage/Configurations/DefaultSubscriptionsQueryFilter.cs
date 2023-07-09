using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skol.Messaging.Ingress.Domain.Models;

namespace Skol.Messaging.Ingress.Internals.Storage.Configurations
{
    internal sealed class DefaultSubscriptionsQueryFilter : EntityTypeConfiguration<Subscription>
    {
        public override void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.HasQueryFilter(s => !s.Archived);
        }
    }
}
