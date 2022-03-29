#region Imports
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
#endregion

namespace AzureAppConfigurationLabs.Web
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
                        var credential = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == EnvironmentName.Development
                            ? new DefaultAzureCredential()
                            : (TokenCredential)new ManagedIdentityCredential();
                    
                        if (!context.HostingEnvironment.IsDevelopment())
                        {
                            // Way-1: Secure way (For Production scenario, works with Azure Services like App Service, VM, VMSS, Functions)
                            // Connect to Azure App Configuration using the Managed Identity
                            // Prerequisite: The application identity is granted "App Configuration Data Reader" role in the App Configuration store
                            
                            var appConfigurationEndpoint = settings["AzureAppConfigurationEndpoint"];

                            if (!string.IsNullOrEmpty(appConfigurationEndpoint))
                            {
                                config.AddAzureAppConfiguration(options =>
                                {
                                    options.Connect(new Uri(appConfigurationEndpoint), credential)
                                    .ConfigureKeyVault(kv =>
                                    {
                                        kv.SetCredential(credential);
                                    };
                                    //    .Select(keyFilter: "MyApp:*")
                                    //    .Select(keyFilter: "Settings:*")
                                    //    .ConfigureRefresh((refreshOptions) =>
                                    //    {
                                    //        // Indicates that all configuration should be refreshed when the given key has changed.
                                    //        refreshOptions.Register(key: "Settings:RefreshRate", refreshAll: true);
                                    //    });
                                    
                                    //// Not recommanded
                                    //options.Connect(new Uri(appConfigurationEndpoint), new ManagedIdentityCredential("YtB9-l0-s0:wLoLgJN/gvBQZKPCZsgc"));
                                });
                            }

                            // OLD: When using version 2.1 in the following way, it works fine
                            //if (!string.IsNullOrEmpty(appConfigurationEndpoint))
                            //{
                            //    config.AddAzureAppConfiguration(options =>
                            //        options.ConnectWithManagedIdentity(appConfigurationEndpoint));
                            //}
                        }
                        else
                        {
                            // Way-2
                            // Connect to Azure App Configuration using the Connection String.
                            var appConfigurationConnectionString = settings["AzureAppConfigurationConnectionString"];
                            //config.AddAzureAppConfiguration(appConfigurationConnectionString);
                            config.AddAzureAppConfiguration(options =>
                            {
                                options.Connect(appConfigurationConnectionString, credential)
                                .ConfigureKeyVault(kv =>
                                {
                                    kv.SetCredential(credential);
                                };
                                .ConfigureRefresh((refreshOptions) =>
                                {
                                    // indicates that all configuration should be refreshed when the given key has changed.
                                    refreshOptions.Register(key: "Settings:Sentinel", refreshAll: true);
                                    refreshOptions.SetCacheExpiration(TimeSpan.FromSeconds(5));
                                })
                                .UseFeatureFlags();
                            });

                            //// Way-3
                            //// Misc: Use App Configuration values as well as Key Vault references.
                            //// You can now access Key Vault references just like any other App Configuration key.
                            //// The config provider will use the KeyVaultClient that you configured to authenticate to Key Vault and retrieve the value.
                            //var azureServiceTokenProvider = new AzureServiceTokenProvider();
                            //var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                            //config.AddAzureAppConfiguration(options => 
                            //    options.Connect(appConfigurationConnectionString)
                            //        .UseAzureKeyVault(keyVaultClient)
                            //    );
                        }
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}



#region Reference

/*
https://github.com/Azure/AppConfiguration
https://github.com/Azure/AppConfiguration/blob/main/examples/DotNetCore/WebDemo/Microsoft.Azure.AppConfiguration.WebDemo/Program.cs
https://docs.microsoft.com/en-us/azure/azure-app-configuration/quickstart-aspnet-core-app
https://docs.microsoft.com/en-us/azure/azure-app-configuration/howto-integrate-azure-managed-service-identity
*/

#endregion
