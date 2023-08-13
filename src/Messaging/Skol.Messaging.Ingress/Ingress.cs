using System.Fabric;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.ServiceFabric.AspNetCore.Configuration;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Skol.Messaging.Ingress;

/// <summary>
/// The FabricRuntime creates an instance of this class for each service type instance.
/// </summary>
internal sealed class Ingress : StatelessService
{
    public Ingress(StatelessServiceContext context) : base(context)
    { }

    /// <summary>
    /// Optional override to create listeners (like tcp, http) for this service instance.
    /// </summary>
    /// <returns>The collection of listeners.</returns>
    protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
    {
        return new ServiceInstanceListener[]
        {
            new ServiceInstanceListener(serviceContext =>
                new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                {
                    ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                    var builder = WebApplication.CreateBuilder();

                    builder.Services.AddSingleton<StatelessServiceContext>(serviceContext);
                    builder.WebHost.UseKestrel(opt =>
                                   {
                                       int port = serviceContext.CodePackageActivationContext.GetEndpoint("ServiceEndpoint").Port;
                                       opt.Listen(IPAddress.IPv6Any, port, listenOptions => listenOptions.UseHttps(https =>
                                       {
                                           https.ServerCertificate = GetCertificateFromStore();
                                           // HINT: Allow the machine to decide.
                                           //https.SslProtocols = SslProtocols.Tls12;
                                       }));
                                   })
                                   .UseContentRoot(Directory.GetCurrentDirectory())
                                   .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                   .UseUrls(url);
                    
                    // Add services to the container.
                    builder.Configuration.AddServiceFabricConfiguration();

                    builder.Logging.ClearProviders()
                                   .AddConsole()
                                   .AddDebug();

                    builder.Services.AddJsonOptions()
                                    .AddIntentsDb()
                                    .AddMessageBroker();

                    var app = builder.Build();
                    
                    // Configure the HTTP request pipeline.
                    app.MapEndpoints();

                    return app;

                }))
        };
    }

    /// <summary>
    /// Finds the ASP .NET Core HTTPS development certificate in development environment. Update this method to use the appropriate certificate for production environment.
    /// </summary>
    /// <returns>Returns the ASP .NET Core HTTPS development certificate</returns>
    private static X509Certificate2 GetCertificateFromStore()
    {
        string aspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (string.Equals(aspNetCoreEnvironment, "Development", StringComparison.OrdinalIgnoreCase))
        {
            const string aspNetHttpsOid = "1.3.6.1.4.1.311.84.1.1";
            const string CNName = "CN=localhost";
            using (X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                var certCollection = store.Certificates;
                var currentCerts = certCollection.Find(X509FindType.FindByExtension, aspNetHttpsOid, true);
                currentCerts = currentCerts.Find(X509FindType.FindByIssuerDistinguishedName, CNName, true);
                return currentCerts.Count == 0 ? null : currentCerts[0];
            }
        }
        else
        {
            throw new NotImplementedException("GetCertificateFromStore should be updated to retrieve the certificate for non Development environment");
        }
    }
}
