using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Router.Enums
{
    public enum AuthorizationType
    {
        None = 0,
        APIKey = 1,
        Bearer = 2,
        Basic = 3,
        OAuth = 4,
    }
}
