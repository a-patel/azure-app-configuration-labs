#region Imports
using AzureAppConfigurationLabs.Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
#endregion

namespace AzureAppConfigurationLabs.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        #region Members

        private readonly Settings _settings;

        #endregion

        #region Ctor

        public CustomerController(IOptionsSnapshot<Settings> options)
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

        #endregion
    }
}