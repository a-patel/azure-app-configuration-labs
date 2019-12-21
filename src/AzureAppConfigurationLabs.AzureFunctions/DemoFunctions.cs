#region Imports
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
#endregion

namespace AzureAppConfigurationLabs.AzureFunctions
{
    public static class DemoFunctions
    {
        #region Members

        // Use the static modifier to create a singleton instance of Configuration. This avoids
        // reloading of configuration for every Azure Function call.
        // The configuration will be cached and can be refreshed based on customization.
        private static IConfiguration Configuration { set; get; }

        private static IConfigurationRefresher ConfigurationRefresher { set; get; }

        #endregion

        #region Ctor

        static DemoFunctions()
        {
            var builder = new ConfigurationBuilder();

            builder.AddAzureAppConfiguration(options =>
            {
                options.Connect(Environment.GetEnvironmentVariable("ConnectionString"))
                    // Configure to reload all configuration if the 'Sentinel' key is modified and
                    // set cache expiration time window to 1 minute.
                    .ConfigureRefresh(refreshOptions =>
                        refreshOptions.Register("Sentinel", refreshAll: true)
                            .SetCacheExpiration(TimeSpan.FromSeconds(60))
                    );

                ConfigurationRefresher = options.GetRefresher();
            });

            Configuration = builder.Build();
        }

        #endregion

        #region Functions

        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Signal to refresh the configuration if the 'Sentinel' key is modified. This will be no-op
            // if the cache expiration time window is not reached.
            // Remove the 'await' operator if the configuration is preferred to be refreshed without blocking.
            await ConfigurationRefresher.Refresh();

            var keyName = "TestApp:Settings:Message";
            var message = Configuration[keyName];

            return message != null
                ? (ActionResult)new OkObjectResult(message)
                : new BadRequestObjectResult($"Please create a key-value with the key '{keyName}' in App Configuration.");
        }

        #endregion
    }
}
