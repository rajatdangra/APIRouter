using System.Threading.Tasks;

namespace Router.Interfaces
{
    public interface ITokenService
    {
        Task<string> GetToken(string userName, string password);
    }
}
