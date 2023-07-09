using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skol.Resthooks.Subs.Domain.Models;

namespace Skol.Resthooks.Subs.Internals.Storage.Configurations
{
    internal sealed class DefaultTopicsQueryFilter : EntityTypeConfiguration<Topic>
    {
        public override void Configure(EntityTypeBuilder<Topic> builder)
        {
            builder.HasQueryFilter(t => !t.Archived);
        }
    }
}
