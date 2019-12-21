#region Imports
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
            
            // Finish on key press
            Console.ReadKey();
            cts.Cancel();
        }

    }
}
