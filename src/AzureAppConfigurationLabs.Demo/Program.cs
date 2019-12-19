#region Imports
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
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
                        //config.AddAzureAppConfiguration(appConfigurationConnectionString);


                        // Way-2
                        // Connect to Azure App Configuration using the Managed Identity
                        var appConfigurationEndpoint = settings["AzureAppConfiguration:Endpoint"];

                        //if (!string.IsNullOrEmpty(appConfigurationEndpoint))
                        //{
                        //    config.AddAzureAppConfiguration(options =>
                        //    {
                        //        options.Connect(new Uri(appConfigurationEndpoint), new ManagedIdentityCredential("YtB9-l0-s0:wLoLgJN/gvBQZKPCZsgc"));
                        //        //options.Connect(new Uri(appConfigurationEndpoint), new DefaultAzureCredential())
                        //        //    .Select(keyFilter: "Settings:*")
                        //        //    .ConfigureRefresh((refreshOptions) =>
                        //        //    {
                        //        //        // Indicates that all configuration should be refreshed when the given key has changed.
                        //        //        refreshOptions.Register(key: "Settings:RefreshRate", refreshAll: true);
                        //        //    });
                        //    });
                        //}

                        // OLD: When using version 2.1 in the following way, it works fine
                        //if (!string.IsNullOrEmpty(appConfigurationEndpoint))
                        //{
                        //    config.AddAzureAppConfiguration(options =>
                        //        options.ConnectWithManagedIdentity(appConfigurationEndpoint));
                        //}


                        // Way-3
                        // Misc: Use App Configuration values as well as Key Vault references.
                        // You can now access Key Vault references just like any other App Configuration key.
                        // The config provider will use the KeyVaultClient that you configured to authenticate to Key Vault and retrieve the value.
                        var azureServiceTokenProvider = new AzureServiceTokenProvider();
                        var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                        config.AddAzureAppConfiguration(options => 
                            options.Connect(appConfigurationConnectionString)
                                .UseAzureKeyVault(keyVaultClient)
                            );
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}



#region Reference

/*
https://docs.microsoft.com/en-us/azure/azure-app-configuration/quickstart-aspnet-core-app
https://docs.microsoft.com/en-us/azure/azure-app-configuration/howto-integrate-azure-managed-service-identity
*/

#endregion
