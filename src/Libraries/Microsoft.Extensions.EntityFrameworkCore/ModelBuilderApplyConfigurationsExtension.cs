namespace Microsoft.EntityFrameworkCore
{
    public static class ModelBuilderApplyConfigurationsExtension
    {
        public static ModelBuilder ApplyConfigurations(this ModelBuilder modelBuilder, params EntityTypeConfiguration[] configurations)
        {
            foreach (EntityTypeConfiguration mapper in configurations)
            {
                mapper.Configure(modelBuilder);
            }

            return modelBuilder;
        }
    }
}
