using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skol.Messaging.Ingress.Domain.Models;

namespace Skol.Messaging.Ingress.Internals.Storage.Configurations;

internal sealed class TopicMappingConfiguration : EntityTypeConfiguration<Topic>
{
    public override void Configure(EntityTypeBuilder<Topic> builder)
    {
        builder.HasQueryFilter(t => !t.Archived);
    }
}
