#region Imports
using AzureAppConfigurationLabs.Demo.Extensions;
using AzureAppConfigurationLabs.Demo.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
#endregion

namespace AzureAppConfigurationLabs.Demo
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // add a Settings model to the service container, which takes its values from the applications configuration.
            services.Configure<Settings>(Configuration.GetSection("Settings"));

            services.AddFeatureManagement();

            services.AddCustomSwagger();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable automatic configuration refresh from Azure App Configuration
            app.UseAzureAppConfiguration();

            app.UseRouting();

            app.UseCustomSwagger();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
