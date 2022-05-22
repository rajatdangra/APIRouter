using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Router.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Router.Services
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        #region Property  
        private readonly IOptions<RouterConfig> _routerSettings;
        private readonly IUserService _userService;
        #endregion

        #region Constructor  
        public BasicAuthenticationHandler(IUserService userService, IOptions<RouterConfig> routerSettings,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _userService = userService;
            _routerSettings = routerSettings;
        }
        #endregion

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string username = null;
            try
            {
                bool isAuthenticationDisabled = _routerSettings.Value.DisableAuthentication;
                if (!isAuthenticationDisabled)
                {
                    var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                    var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':');
                    username = credentials.FirstOrDefault();
                    var password = credentials.LastOrDefault();

                    if (!_userService.ValidateCredentials(username, password))
                        throw new ArgumentException("Invalid credentials");
                }
                else
                {
                    username = "Authentication Disabled";
                }
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
            }

            var claims = new[] {
                new Claim(ClaimTypes.Name, username)
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

    }
}
