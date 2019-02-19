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

	    public UsersService(IdentityContext context, ILoggerFactory loggerFactory)
	    {
	        _context = context ?? throw new ArgumentNullException(nameof(context));

	        if (loggerFactory == null)
	        {
	            throw new ArgumentNullException(nameof(loggerFactory));
	        }

	        _logger = loggerFactory.CreateLogger<UsersService>();
	    }

        public async Task CreateUser(User user)
        {
            try
            {
                var userEntity = MapUserToDomainModel(user);

                await _context.Users.AddAsync(userEntity);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to add user - exception: '{e.Message}' userId: '{user.UserId}'");

                throw new Exception("Failed to add user");
            }
        }

        private Users MapUserToDomainModel(User user)
        {
            // TODO: Check required fields

            var userEntity = new Users()
            {
                AcceptedTerms = true,
                AllowContactMe = true,
                Auth0UserId = user.UserId,
                FirstName = user.FirstName,
                InitialRegistrationDate = DateTime.UtcNow,
                EmailAddress = user.Email
            };

            return userEntity;
        }
    }
}