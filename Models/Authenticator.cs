using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Router.Enums;

namespace Router.Models
{
    public class Authenticator
    {
        public TokenType TokenType { get; set; }
        public string ApiKey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsEncode { get; set; }

        //Converts API Key to Base64 
        public static string Encoding(string toEncode)
        {
            byte[] bytes = System.Text.Encoding.Default.GetBytes(toEncode);
            string toReturn = Convert.ToBase64String(bytes);
            return toReturn;
        }
    }
}
