using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Router.Configuration
{
    public class RouterConfig
    {
        //public string URL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool DisableAuthentication { get; set; }
        public int HttpConnectionTimeOut { get; set; }
        public bool DisableLogging { get; set; }
    }
}
