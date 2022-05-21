using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Router.Configuration
{
    public class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "1";
        public const string Base = Root + "/" + Version;

        public static class Router
        {
            public const string Route = Base + "/Route";
        }
    }
}
