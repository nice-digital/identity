using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Domain;

namespace NICE.Identity.Authentication.Sdk.TokenStore
{
    public interface IApiTokenStore
    {
        Task<string> StoreAsync(JwtToken token);
        Task RenewAsync(string key, JwtToken token);
        Task<JwtToken> RetrieveAsync(string key);
        Task RemoveAsync(string key);
    }
}
