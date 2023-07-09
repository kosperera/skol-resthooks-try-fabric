using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skol.Resthooks.Subs.Domain.Models;

namespace Skol.Resthooks.Subs.Internals.Storage.Configurations
{
    internal sealed class TopicMappingConfiguration : EntityTypeConfiguration<Topic>
    {
        public override void Configure(EntityTypeBuilder<Topic> builder)
        {
            builder.Property(t => t.Id).IsRequired()
                                       .ValueGeneratedNever();
        }
    }
}
