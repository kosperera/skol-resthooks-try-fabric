using System.Diagnostics;
using Microsoft.ServiceFabric.Services.Runtime;
using Skol.Resthooks.Subs;

try
{
    // The ServiceManifest.XML file defines one or more service type names.
    // Registering a service maps a service type name to a .NET type.
    // When Service Fabric creates an instance of this service type,
    // an instance of the class is created in this host process.

    ServiceRuntime.RegisterServiceAsync("Skol.Resthooks.SubsType", context => new Subs(context))
                  .GetAwaiter()
                  .GetResult();

    ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(Subs).Name);

    // Prevents this host process from terminating so services keeps running. 
    Thread.Sleep(Timeout.Infinite);
}
catch (Exception e)
{
    ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
    throw;
}
