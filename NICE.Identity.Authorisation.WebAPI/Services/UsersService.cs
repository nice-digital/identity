using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.ApiModels.Requests;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using IdentityContext = NICE.Identity.Authorisation.WebAPI.Repositories.IdentityContext;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IUsersService
    {
		Task CreateUser(CreateUser user);
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

        public async Task CreateUser(CreateUser user)
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

        private DataModels.User MapUserToDomainModel(CreateUser user)
        {           
            var userEntity = new User
            {
                //AcceptedTerms = user.AcceptedTerms,
                //TODO: AllowContactMe = user.AllowContactMe,
                Auth0UserId = user.UserId,
                FirstName = user.FirstName,
				LastName = user.LastName,
                InitialRegistrationDate = DateTime.UtcNow,
                EmailAddress = user.Email,
				IsLockedOut = false,
				HasVerifiedEmailAddress = false
            };
            if (user.AcceptedTerms)
            {
                var currentTerms = _context.GetLatestTermsVersion();
                if (currentTerms != null)
                {
                    userEntity.UserAcceptedTermsVersions = new List<UserAcceptedTermsVersion>() { new UserAcceptedTermsVersion(currentTerms) };
                }
            }

            return userEntity;
        }
    }
}