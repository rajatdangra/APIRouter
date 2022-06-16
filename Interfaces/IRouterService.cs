using Router.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Router.Interfaces
{
    public interface IRouterService
    {
        Task<string> CRUDOperations(string url, string methodType, Authenticator authenticator, List<Header> headers, object jsonBody = null, bool isCheckErrorResponse = true);
        //Task<IResponseEntity> GetCSRFToken(string guid, string url, int apiType);
    }
}
