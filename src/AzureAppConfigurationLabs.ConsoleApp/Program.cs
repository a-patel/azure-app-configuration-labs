﻿#region Imports
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace AzureAppConfigurationLabs.ConsoleApp
{
    class Program
    {
        static IConfiguration Configuration { get; set; }
        static IConfigurationRefresher _refresher;

        async static Task Main(string[] args)
        {
            Configure();

            var cts = new CancellationTokenSource();
            await Run(cts.Token);

            // Finish on key press
            Console.ReadKey();
            cts.Cancel();
        }

        private static void Configure()
        {
            var builder = new ConfigurationBuilder();

            // Load a subset of the application's configuration from a json file and environment variables
            builder.AddJsonFile("appsettings.json")
                   .AddEnvironmentVariables();

            var configuration = builder.Build();

            if (string.IsNullOrEmpty(configuration["ConnectionString"]))
            {
                Console.WriteLine("Connection string not found.");
                Console.WriteLine("Please set the 'ConnectionString' environment variable to a valid Azure App Configuration connection string and re-run this example.");
                return;
            }

            // Augment the configuration builder with Azure App Configuration
            // Pull the connection string from an environment variable
            builder.AddAzureAppConfiguration(options =>
            {
                options.Connect(configuration["ConnectionString"])
                       .Select("Settings:AppName")
                       .Select("Settings:BackgroundColor")
                       .ConfigureRefresh(refresh =>
                       {
                           refresh.Register("Settings:AppName")
                                  .Register("Language", refreshAll: true)
                                  .SetCacheExpiration(TimeSpan.FromSeconds(10));
                       });

                // Get an instance of the refresher that can be used to refresh data
                _refresher = options.GetRefresher();
            });

            Configuration = builder.Build();
        }

        private static async Task Run(CancellationToken token)
        {
            var display = string.Empty;
            var sb = new StringBuilder();

            do
            {
                sb.Clear();

                // Trigger and wait for an async refresh for registered configuration settings
                await _refresher.RefreshAsync();

                sb.AppendLine($"{Configuration["Settings:AppName"]} has been configured to run in {Configuration["Language"]}");
                sb.AppendLine();

                sb.AppendLine(string.Equals(Configuration["Language"], "spanish", StringComparison.OrdinalIgnoreCase) ? "Buenos Dias." : "Good morning");
                sb.AppendLine();

                sb.AppendLine("Press any key to exit...");

                display = sb.ToString();

                Console.Clear();
                Console.Write(display);

                await Task.Delay(1000);
            } while (!token.IsCancellationRequested);
        }
    }
}
