using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public interface IProviderManagementService
    {
        Task DeleteUser(string authenticationProviderUserId);
    }
}