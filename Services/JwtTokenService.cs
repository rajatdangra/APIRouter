using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Router.Configuration;
using Router.Interfaces;
using Router.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Router.Services
{
    public class JwtTokenService : ITokenService
    {
        private readonly IOptions<TokenConfig> _tokenSettings;
        private readonly IUserService _userService;
        public JwtTokenService(IOptions<TokenConfig> tokenSettings, IUserService userService)
        {
            _tokenSettings = tokenSettings;
            _userService = userService;
        }

        public async Task<string> GetToken(string userName, string password)
        {
            try
            {
                if (_userService.ValidateCredentials(userName, password))
                {
                    //create claims details based on the user information
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _tokenSettings.Value.Subject),
                        //new Claim(JwtRegisteredClaimNames.Typ, _tokenSettings.Value.Type),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserName", userName),
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Value.Key));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _tokenSettings.Value.Issuer,
                        _tokenSettings.Value.Audience,
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(_tokenSettings.Value.Expiry),
                        signingCredentials: signIn);

                    return new JwtSecurityTokenHandler().WriteToken(token);
                }
                else
                {
                    //return BadRequest("Invalid Credentials!");
                    throw new UnauthorizedAccessException("Invalid Credentials!");
                }
            }
            catch (Exception ex)
            {
                //_logger.Info("token, userid and sourcetype is blank");
                throw new UnauthorizedAccessException("$Authentication failed: {ex.Message}");
            }
        }
    }
}
