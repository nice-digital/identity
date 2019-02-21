using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.ApiModels.Requests;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using IdentityContext = NICE.Identity.Authorisation.WebAPI.Repositories.IdentityContext;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IUsersService
    {
		Task CreateUser(User user);
	}

	public class UsersService : IUsersService
    {
		private readonly IdentityContext _context;
	    private readonly ILogger<UsersService> _logger;

	    public UsersService(IdentityContext context, ILogger<UsersService> logger)
	    {
	        _context = context ?? throw new ArgumentNullException(nameof(context));
	        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
	    }

        public async Task CreateUser(User user)
        {
            try
            {
                var userEntity = MapUserToDomainModel(user);

                _context.AddUser(userEntity);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to add user - exception: '{e.Message}' userId: '{user.UserId}'");

                throw new Exception("Failed to add user");
            }
        }

        private Users MapUserToDomainModel(User user)
        {           
            var userEntity = new Users
            {
                AcceptedTerms = user.AcceptedTerms,
                //TODO: AllowContactMe = user.AllowContactMe,
                Auth0UserId = user.UserId,
                FirstName = user.FirstName,
				LastName = user.LastName,
                InitialRegistrationDate = DateTime.UtcNow,
                EmailAddress = user.Email,
				IsLockedOut = false,
				HasVerifiedEmailAddress = false
            };

            return userEntity;
        }
    }
}