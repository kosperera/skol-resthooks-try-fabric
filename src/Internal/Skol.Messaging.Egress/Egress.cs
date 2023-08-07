using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Skol.Messaging.Egress;

/// <summary>
/// An instance of this class is created for each service instance by the Service Fabric runtime.
/// </summary>
internal sealed class Egress : StatelessService
{
    public Egress(StatelessServiceContext context) : base(context)
    { }

    /// <summary>
    /// This is the main entry point for your service instance.
    /// </summary>
    /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
    protected override async Task RunAsync(CancellationToken cancellationToken)
    {
        var builder = Host.CreateApplicationBuilder();

        // Add services to the container.

        builder.Configuration.AddFabricConfiguration("Config");

        builder.Logging.ClearProviders()
                       .AddConsole()
                       .AddDebug();

        builder.Services.AddJsonOptions()
                        .AddWebhookProxy()
                        .AddMessageBroker();

        var app = builder.Build();

        await app.RunAsync(cancellationToken);
    }
}
