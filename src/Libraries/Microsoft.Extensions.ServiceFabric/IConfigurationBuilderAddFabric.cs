using System.Fabric;
using System.Fabric.Description;
using Microsoft.Extensions.ServiceFabric.Internals;

namespace Microsoft.Extensions.Configuration
{
    public static class IConfigurationBuilderAddFabric
    {
        public static IConfigurationBuilder AddFabricConfiguration(this IConfigurationBuilder builder, string packageName)
            => builder.AddFabricConfiguration(FabricRuntime.GetActivationContext()
                                                           .GetConfigurationPackageObject(packageName)
                                                           .Settings);

        public static IConfigurationBuilder AddFabricConfiguration(this IConfigurationBuilder builder, ConfigurationSettings source)
            => builder.Add(new FabricConfigurationSource(source));
    }
}
