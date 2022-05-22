using Microsoft.Extensions.Options;
using Router.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Router.Services
{
    public class UserService : IUserService
    {
        private readonly IOptions<RouterConfig> _routerSettings;
        public UserService(IOptions<RouterConfig> routerSettings)
        {
            _routerSettings = routerSettings;
        }
        public bool ValidateCredentials(string username, string password)
        {
            bool isValid = username.Equals(_routerSettings.Value.Username) && password.Equals(_routerSettings.Value.Password);
            return isValid;
        }
    }
}
