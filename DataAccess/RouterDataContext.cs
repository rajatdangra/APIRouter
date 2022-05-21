using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Router.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Net;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Router.Models;
using Router.Enums;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Interfaces;

namespace Router.DataAccess.DataContext
{
    public class RouterDataContext : IRouterDataContext
    {
        private readonly IOptions<RouterConfig> _routerSettings;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<RouterDataContext> _logger;

        public RouterDataContext(IHttpClientFactory clientFactory, IOptions<RouterConfig> routerSettings, ILogger<RouterDataContext> logger)
        {
            _routerSettings = routerSettings;
            _clientFactory = clientFactory;
            _logger = logger;
        }
        #region private properties
        public string Result = string.Empty;
        public StringContent InputData;
        public JObject ResponseResult;
        public HttpResponseMessage Response;
        public readonly CancellationToken cancellationToken;
        #endregion


        public async Task<string> CRUDOperations(string url, string methodType, Authenticator authenticator, List<Header> headers, object jsonBody = null, bool isCheckErrorResponse = true)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Tls13;
                StringBuilder sbRouter = new StringBuilder();
                sbRouter.Append($"Router URL: {url} {Environment.NewLine}");
                sbRouter.Append($"Method: {methodType} {Environment.NewLine}");

                APIMethodEnum apiMethodType = (APIMethodEnum)Enum.Parse(typeof(APIMethodEnum), methodType);

                var httpClient = _clientFactory.CreateClient("RouterClient");
                httpClient.Timeout = TimeSpan.FromMinutes(_routerSettings.Value.HttpConnectionTimeOut);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                if (authenticator != null)
                {
                    switch (authenticator.TokenType)
                    {
                        case TokenType.None:
                            break;
                        case TokenType.Bearer:
                            authenticator.ApiKey = authenticator.IsEncode ? Authenticator.Encoding(authenticator.ApiKey) : authenticator.ApiKey;
                            httpClient.DefaultRequestHeaders.Add("Authorization", $"{TokenType.Bearer} {authenticator.ApiKey}");
                            break;
                        case TokenType.Basic:
                            string apikey = $"{authenticator.Username}:{authenticator.Password}";
                            apikey = authenticator.IsEncode ? Authenticator.Encoding(apikey) : apikey;
                            httpClient.DefaultRequestHeaders.Add("Authorization", $"{TokenType.Basic} {apikey}");
                            break;
                    }
                }

                sbRouter.Append($"Request Header Sent: {httpClient.DefaultRequestHeaders}{ Environment.NewLine}");
                _logger.LogWarning(sbRouter.ToString());

                if (apiMethodType == APIMethodEnum.POST || apiMethodType == APIMethodEnum.PUT || apiMethodType == APIMethodEnum.PATCH)
                {
                    var jsonString = JsonConvert.SerializeObject(jsonBody);
                    if (!_routerSettings.Value.DisableLogging)
                        sbRouter.Append($"Payload Sent: {JsonConvert.SerializeObject(jsonBody, Formatting.Indented) } {Environment.NewLine }");
                    InputData = new StringContent(jsonString, Encoding.UTF8, "application/json");
                }

                switch (apiMethodType)
                {
                    case APIMethodEnum.GET:
                        Response = await httpClient.GetAsync(url, cancellationToken);
                        break;
                    case APIMethodEnum.POST:
                        Response = await httpClient.PostAsync(url, InputData, cancellationToken);
                        break;
                    case APIMethodEnum.PUT:
                        Response = await httpClient.PutAsync(url, InputData, cancellationToken);
                        break;
                    case APIMethodEnum.PATCH:
                        Response = await httpClient.PatchAsync(url, InputData, cancellationToken);
                        break;
                    case APIMethodEnum.DELETE:
                        Response = await httpClient.DeleteAsync(url, cancellationToken);
                        break;
                    default:
                        break;
                }
                sbRouter.Append($"Response Header Recieved: {Response?.Headers}{ Environment.NewLine}");
                sbRouter.Append($"Response Status Code Recieved: {Response?.StatusCode}{ Environment.NewLine}");
                if (!_routerSettings.Value.DisableLogging)
                    sbRouter.Append($"Router Response : {Response?.Content.ReadAsStringAsync(cancellationToken).Result}{ Environment.NewLine}");
                _logger.LogInformation(sbRouter.ToString());
                if (isCheckErrorResponse)
                    Response.EnsureSuccessStatusCode();
                Result = Response?.Content.ReadAsStringAsync(cancellationToken).Result;
            }
            catch (Exception exception) when (exception is WebException || exception is HttpRequestException || exception is SocketException)
            {
                _logger.LogInformation($"Router Failed Response : {Response?.Content.ReadAsStringAsync(cancellationToken).Result}");
                if (exception is IOException)
                {
                    _logger.LogInformation("Exception IOException details : " + exception);
                }
                else if (exception is CommunicationException)
                {
                    _logger.LogInformation("Exception CommunicationException details : " + exception);
                }
                else
                {
                    _logger.LogInformation("Exception details : " + exception);
                    //_logger.LogError("Router Response: " + response?.Content.ReadAsStringAsync().Result);
                    if (Response?.StatusCode != null)
                        exception.Data["StatusCode"] = (int)Response?.StatusCode;
                    if (Response != null)
                    {
                        var errorResult = Response?.Content.ReadAsStringAsync(cancellationToken).Result;
                        ResponseResult = JsonConvert.DeserializeObject<JObject>(Convert.ToString(errorResult));
                    }
                }

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Router Failed Response : {ex}");
            }
            return Convert.ToString(Result);
        }
    }
}
