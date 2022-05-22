using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Router.Configuration;
using Router.Services;
using Router.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Router.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class APIRouterController : ControllerBase
    {
        private readonly IRouterService _routerService;
        private readonly IOptions<RouterConfig> _routerSettings;
        private readonly ILogger<APIRouterController> _logger;

        public APIRouterController(ILogger<APIRouterController> logger, IRouterService routerService, IOptions<RouterConfig> routerSettings)
        {
            _logger = logger;
            _routerService = routerService;
            _routerSettings = routerSettings;
        }

        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    var rng = new Random();
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = rng.Next(-20, 55),
        //        Summary = Summaries[rng.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

        [HttpPost(ApiRoutes.Router.Route)]
        public async Task<IActionResult> Route([BindRequired, FromBody] RouterRequest routerRequest)
        {
            object jsonData = null;
            try
            {
                jsonData = routerRequest.JSONBody != null ? JObject.Parse(Convert.ToString(routerRequest.JSONBody)) : routerRequest.JSONBody;
            }
            catch
            {
                _logger.LogInformation($"Invalid JSONBody: {routerRequest.JSONBody}");
            }

            _logger.LogInformation($"JSON Date: {jsonData?.ToString()}");

            string otherURL = string.Empty;
            var response = await _routerService.CRUDOperations(routerRequest.URL, routerRequest.MethodType, routerRequest.Authenticator, routerRequest.Headers, jsonData/*, routerRequest.GUID, routerRequest.ApiType, routerRequest.CreateCSRFCreation, routerRequest.isCheckErrorResponse, otherURL, null*/);
            var responseResult = JsonConvert.DeserializeObject<ExpandoObject>(Convert.ToString(response) ?? string.Empty);
            return new OkObjectResult(responseResult);
        }
    }
}
