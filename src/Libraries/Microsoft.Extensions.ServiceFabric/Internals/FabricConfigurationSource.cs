using System.Fabric.Description;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.ServiceFabric.Internals;

internal sealed class FabricConfigurationSource : IConfigurationSource
{
    readonly ConfigurationSettings _source;

    public FabricConfigurationSource(ConfigurationSettings source)
    {
        _source = source;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new FabricConfigurationProvider(_source);
}
