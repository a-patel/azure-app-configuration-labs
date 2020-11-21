# Azure App Configuration (with .NET) Labs

> Demo: Azure App Configuration + .NET 5.x





Please refer to below articles of my publication [Awesome Azure](https://medium.com/awesome-azure) and [ASPNETCore](https://medium.com/aspnetcore) on Azure App Configuration:

- [Use Azure App Configuration with .NET/ASP.NET Core Applications](https://medium.com/aspnetcore/use-azure-app-configuration-with-asp-net-core-application-6bdf0cc851e2)
- [Azure — Introduction to Azure App Configuration](https://medium.com/awesome-azure/azure-introduction-to-azure-app-configuration-f4a4c43ec5db)



---



## Basic Usage :page_facing_up:

### Step 1 : Install the package :package:

> To install NuGet, run the following command in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console)

```Powershell
PM> Install-Package Microsoft.Azure.AppConfiguration.AspNetCore
PM> Install-Package Azure.Identity
```


### Step 2 : Configuration 🔨

> Here are samples that show you how to config.

##### 2.1 : AppSettings

```js
{
  // Way-1: Connect to Azure App Configuration using the Managed Identity
  "AzureAppConfigurationEndpoint": "https://[YOUR_APP_CONFIGURATION_NAME].azconfig.io",

  // Way-2: Connect to Azure App Configuration using the Connection String
  "AzureAppConfigurationConnectionString": "[YOUR_APP_CONFIGURATION_CONNECTIONSTRING]",

  // Sample Settings of your application
  "Settings": {
    "AppName": "Azure App Configuration Labs",
    "Version": 1.0,
    "FontSize": 50,
    "RefreshRate": 1000,
    "Language": "English",
    "Messages": "Hello There. Thanks for using Azure App Configuration.",
    "BackgroundColor": "Black"
  }
}
```

##### 2.2 : Configure Program Class

```cs
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
                                options.Connect(new Uri(appConfigurationEndpoint), new DefaultAzureCredential());
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
                            options.Connect(appConfigurationConnectionString).ConfigureRefresh((refreshOptions) =>
                            {
                                // indicates that all configuration should be refreshed when the given key has changed.
                                refreshOptions.Register(key: "Settings:Sentinel", refreshAll: true);
                                refreshOptions.SetCacheExpiration(TimeSpan.FromSeconds(5));
                            }).UseFeatureFlags();
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
```


### Step 3 : Use in Controller or Business layer :memo:

```cs
public class TestController : ControllerBase
{
    #region Members

    private readonly Settings _settings;

    #endregion

    #region Ctor

    public TestController(IOptionsSnapshot<Settings> options)
    {
        _settings = options.Value;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get Settings
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("get-settings")]
    public IActionResult GetSettings()
    {
        return Ok(_settings);
    }

    /// <summary>
    /// Get Settings
    /// </summary>
    /// <returns></returns>
    [FeatureGate(FeatureFlags.Beta)]
    [HttpGet]
    [Route("get-beta-settings")]
    public IActionResult GetBetaSettings()
    {
        return Ok("This is Beta feature only");
    }

    #endregion
}
```





---





## Give a Star! :star:

Feel free to request an issue on github if you find bugs or request a new feature. Your valuable feedback is much appreciated to better improve this project. If you find this useful, please give it a star to show your support for this project.


## Support :telephone:

> Reach out to me at one of the following places!

- Email :envelope: at <a href="mailto:toaashishpatel@gmail.com" target="_blank">`toaashishpatel@gmail.com`</a>


## Author :boy:

* **Ashish Patel** - [A-Patel](https://github.com/a-patel)


##### Connect with me

| Linkedin | Portfolio | Medium | GitHub | NuGet | Microsoft | Twitter | Facebook | Instagram |
|----------|----------|----------|----------|----------|----------|----------|----------|----------|
| [![linkedin](https://img.icons8.com/ios-filled/96/000000/linkedin.png)](https://www.linkedin.com/in/iamaashishpatel) | [![Portfolio](https://img.icons8.com/wired/96/000000/domain.png)](https://aashishpatel.netlify.app/) | [![medium](https://img.icons8.com/ios-filled/96/000000/medium-monogram.png)](https://iamaashishpatel.medium.com) | [![github](https://img.icons8.com/ios-glyphs/96/000000/github.png)](https://github.com/a-patel) | [![nuget](https://img.icons8.com/windows/96/000000/nuget.png)](https://nuget.org/profiles/iamaashishpatel) | [![microsoft](https://img.icons8.com/ios-filled/90/000000/microsoft.png)](https://docs.microsoft.com/en-us/users/iamaashishpatel) | [![twitter](https://img.icons8.com/ios-filled/96/000000/twitter.png)](https://twitter.com/aashish_mrcool) | [![facebook](https://img.icons8.com/ios-filled/90/000000/facebook.png)](https://www.facebook.com/aashish.mrcool) | [![instagram](https://img.icons8.com/ios-filled/90/000000/instagram-new.png)](https://www.instagram.com/iamaashishpatel/) |


## Donate :dollar:

If you find this project useful — or just feeling generous, consider buying me a beer or a coffee. Cheers! :beers: :coffee:
| PayPal | BMC | Patreon |
| ------------- | ------------- | ------------- |
| [![PayPal](https://www.paypalobjects.com/webstatic/en_US/btn/btn_donate_pp_142x27.png)](https://www.paypal.me/iamaashishpatel) | [![Buy Me A Coffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/iamaashishpatel) | [![Patreon](https://c5.patreon.com/external/logo/become_a_patron_button.png)](https://www.patreon.com/iamaashishpatel) |


## License :lock:

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
