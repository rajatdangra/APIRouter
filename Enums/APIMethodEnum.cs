using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Router.Enums
{
    public enum APIMethodEnum
    {
        [Description("GET")]
        GET,
        [Description("POST")]
        POST,
        [Description("PUT")]
        PUT,
        [Description("PATCH")]
        PATCH,
        [Description("DELETE")]
        DELETE
    }
}
