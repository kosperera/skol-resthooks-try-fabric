namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionAddEndpointsExtension
    {
        public static IServiceCollection AddEndpoints(this IServiceCollection services)
        {
            services.AddControllers(cfg => cfg.SuppressAsyncSuffixInActionNames = false);

            return services;
        }
    }
}
