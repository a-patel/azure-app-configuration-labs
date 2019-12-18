#region Imports
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
#endregion

namespace AzureAppConfigurationLabs.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((context, config) =>
                    {
                        var settings = config.Build();

                        // Way-1
                        // Connect to Azure App Configuration using the Connection String.
                        var appConfigurationConnectionString = settings["AzureAppConfiguration:ConnectionString"];
                        config.AddAzureAppConfiguration(appConfigurationConnectionString);
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}
