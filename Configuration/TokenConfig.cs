using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Router.Configuration
{
    public class TokenConfig
    {
        public string Type { get; set; }
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }
        public int Expiry { get; set; }
    }
}
