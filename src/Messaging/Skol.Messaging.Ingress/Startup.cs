using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Skol.Messaging.Ingress
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            Configuration = new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                                                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                                                      .AddFabricConfiguration("Config")
                                                      .AddEnvironmentVariables()

                                                      .Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(cfg => cfg.AddConsole().AddDebug())
                    .AddJsonOptions()
                    .AddIntentsDb()
                    .AddMessageBroker()
                    .AddEndpoints();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseEndpoints(env);
        }
    }
}
