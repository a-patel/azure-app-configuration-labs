#region Imports
using AzureAppConfigurationLabs.AzureFunctions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using System.Threading.Tasks;
#endregion

namespace AzureAppConfigurationLabs.AzureFunctions
{
    public class DemoFunctions
    {
        #region Members

        private readonly Settings _settings;
        private readonly IFeatureManagerSnapshot _featureManagerSnapshot;
        private readonly IConfigurationRefresher _configurationRefresher;

        #endregion

        #region Ctor

        public DemoFunctions(IOptionsSnapshot<Settings> settings, IFeatureManagerSnapshot featureManagerSnapshot, IConfigurationRefresher configurationRefresher)
        {
            _settings = settings.Value;
            _featureManagerSnapshot = featureManagerSnapshot;
            _configurationRefresher = configurationRefresher;
        }

        #endregion

        #region Functions

        [FunctionName("GetSettings")]
        public async Task<IActionResult> GetSettings(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // The configuration is refreshed asynchronously without blocking the execution of the current function.
            _ = await _configurationRefresher.TryRefreshAsync();

            string appName = _settings.AppName;

            return appName != null
                ? (ActionResult)new OkObjectResult(appName)
                : new BadRequestObjectResult($"Please create a key-value with the key 'MyFuncApp:Settings:Message' in Azure App Configuration.");
        }

        [FunctionName("ShowBetaFeature")]
        public async Task<IActionResult> ShowBetaFeature(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Signal to refresh the feature flags from Azure App Configuration.
            // This will be a no-op if the cache expiration time window is not reached.
            // Remove the 'await' operator if it's preferred to refresh without blocking.
            await _configurationRefresher.TryRefreshAsync();

            string featureName = "Beta";
            bool featureEnalbed = await _featureManagerSnapshot.IsEnabledAsync(featureName);

            return featureEnalbed
                ? (ActionResult)new OkObjectResult($"{featureName} feature is On")
                : new BadRequestObjectResult($"{featureName} feature is Off (or the feature flag '{featureName}' is not present in Azure App Configuration).");

        }

        #endregion
    }
}



#region Reference
/*
https://github.com/Azure/AppConfiguration/blob/main/examples/DotNetCore/AzureFunction/FunctionApp/ShowMessage.cs
https://github.com/Azure/AppConfiguration/blob/main/examples/DotNetCore/AzureFunction/FunctionApp/ShowBetaFeature.cs 
*/
#endregion
