using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using Router.Configuration;
using Microsoft.Extensions.Options;
using Router.Interfaces;

namespace Router.Services
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AuthenticationFilter
    {
        private readonly IOptions<RouterConfig> _routerSettings;
        private readonly IUserService _userService;
        private readonly RequestDelegate _next;
        private StringValues authParDetails; private StringValues sourceParDetails;
        public AuthenticationFilter(IUserService userService, IOptions<RouterConfig> routerSettings, RequestDelegate next)
        {
            _userService = userService;
            _routerSettings = routerSettings;
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                bool isAuthenticationDisabled = _routerSettings.Value.DisableAuthentication;
                if (!isAuthenticationDisabled)
                {
                    var headers = httpContext.Request.Headers;
                    var authHeader = AuthenticationHeaderValue.Parse(headers["Authorization"]);
                    var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':');
                    var username = credentials.FirstOrDefault();
                    var password = credentials.LastOrDefault();

                    if (!_userService.ValidateCredentials(username, password))
                    {
                        //_logger.Info("token, userid and sourcetype is blank");
                        httpContext.Response.StatusCode = 401; //UnAuthorized
                        await httpContext.Response.WriteAsync($"Authentication failed: Invalid credentials");
                        return;
                    }
                }
                else
                {
                    //username = "Authentication Disabled";
                }
            }
            catch (Exception ex)
            {
                //_logger.Info("token, userid and sourcetype is blank");
                httpContext.Response.StatusCode = 401; //UnAuthorized
                await httpContext.Response.WriteAsync($"Authentication failed: {ex.Message}");
                return;
            }
            await _next(httpContext);
            return;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthenticationFilterExtensions
    {
        public static IApplicationBuilder UseAuthenticationFilter(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationFilter>();
        }
    }
}
