using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skol.Resthooks.Subs.Domain.Models;

namespace Skol.Resthooks.Subs.Internals.Storage.Configurations
{
    internal sealed class DefaultSubscriptionsQueryFilter : EntityTypeConfiguration<Subscription>
    {
        public override void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.HasQueryFilter(s => !s.Archived);
        }
    }
}
