#region Important
using System;
using AzureAppConfigurationLabs.AzureFunctions.Models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement; 
#endregion

[assembly: FunctionsStartup(typeof(AzureAppConfigurationLabs.AzureFunctions.Startup))]
namespace AzureAppConfigurationLabs.AzureFunctions
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            IConfigurationRefresher configurationRefresher = null;

            // Load configuration from Azure App Configuration
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddAzureAppConfiguration(options =>
            {
                options.Connect(Environment.GetEnvironmentVariable("AzureAppConfigurationConnectionString"))
                       // Load all keys that start with `TestApp:`
                       .Select("MyFuncApp:*")
                       // Configure to reload configuration if the registered 'Sentinel' key is modified
                       .ConfigureRefresh(refreshOptions =>
                            refreshOptions.Register("MyFuncApp:Settings:Sentinel", refreshAll: true)
                        )
                       // Indicate to load feature flags
                       .UseFeatureFlags();
                configurationRefresher = options.GetRefresher();
            });
            IConfiguration configuration = configurationBuilder.Build();

            // Make settings, feature manager and configuration refresher available through DI
            builder.Services.Configure<Settings>(configuration.GetSection("MyFuncApp:Settings"));
            builder.Services.AddFeatureManagement(configuration);
            builder.Services.AddSingleton<IConfigurationRefresher>(configurationRefresher);
        }
    }
}



#region Reference
/*
https://github.com/Azure/AppConfiguration/blob/main/examples/DotNetCore/AzureFunction/FunctionApp/Startup.cs 
*/
#endregion
