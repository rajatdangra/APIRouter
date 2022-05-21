using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Router.Enums
{
    public enum StatusEnum
    {
        [Description("Success")]
        Success = 0,
        [Description("Fail")]
        Fail = 1,
        [Description("AUTH")]
        AUTH
    }
}
