using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Router.Configuration;
using Router.Interfaces;
using Router.Models;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Router.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly ILogger<TokenController> _logger;
        private readonly ITokenService _tokenService;
        public TokenController(ILogger<TokenController> logger, ITokenService tokenService)
        {
            _logger = logger;
            _tokenService = tokenService;
        }


        [AllowAnonymous]
        [HttpPost(ApiRoutes.Token.GetToken)]
        public async Task<IActionResult> GetToken([BindRequired, FromBody] User user)
        {
            try
            {
                //var headers = Request.Headers;
                //var authHeader = AuthenticationHeaderValue.Parse(headers["Authorization"]);
                //var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':');
                //var username = credentials.FirstOrDefault();
                //var password = credentials.LastOrDefault();

                var response = await _tokenService.GetToken(user.UserName, user.Password);
                return new OkObjectResult(Convert.ToString(response));
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Invalid Credentials! Exception: {ex}");
                return new UnauthorizedObjectResult("Invalid Credentials!");
            }
        }
    }
}
