using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Router.Models
{
    public class RouterRequest
    {
        public string URL { get; set; }
        public string MethodType { get; set; } = "GET";
        public object JSONBody { get; set; }
        public Authenticator Authenticator { get; set; }
        public List<Header> Headers { get; set; }

        //public string GUID { get; set; }
        //public int ApiType { get; set; } = 0;
        //public string OtherURL { get; set; }
        //public bool isCheckErrorResponse { get; set; }
        //public bool CreateCSRFCreation { get; set; }

    }
}
