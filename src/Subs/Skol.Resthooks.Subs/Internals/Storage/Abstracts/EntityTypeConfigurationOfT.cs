using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore
{
    internal abstract class EntityTypeConfiguration<TEntity> : EntityTypeConfiguration, IEntityTypeConfiguration<TEntity> where TEntity : class
    {
        public abstract void Configure(EntityTypeBuilder<TEntity> builder);
        public override void Configure(ModelBuilder modelBuilder)
            => Configure(modelBuilder.Entity<TEntity>());
    }
}
