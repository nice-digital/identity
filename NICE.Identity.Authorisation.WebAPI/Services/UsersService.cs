using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Repositories;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public interface IUsersService
    {
        User CreateUser(User user);
        User GetUser(int userId);
        List<User> GetUsers();
        List<UserDetails> FindUsers(IEnumerable<string> auth0UserIds);
        Dictionary<string, IEnumerable<string>> FindRoles(IEnumerable<string> auth0UserIds, string host);
		User UpdateUser(int userId, User user);
        int DeleteUser(int userId);
    }

    public class UsersService : IUsersService
    {
        private readonly IdentityContext _context;
        private readonly ILogger<UsersService> _logger;
        private readonly IProviderManagementService _providerManagementService;

        public UsersService(IdentityContext context, ILogger<UsersService> logger, IProviderManagementService providerManagementService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _providerManagementService = providerManagementService ?? throw new ArgumentNullException(nameof(providerManagementService));
        }

        public User CreateUser(User user)
        {
            try
            {
                var userToCreate = new DataModels.User();
                userToCreate.UpdateFromApiModel(user);
                if (user.AcceptedTerms == true)
                {
                    var currentTerms = _context.GetLatestTermsVersion();
                    if (currentTerms != null)
                    {
                        userToCreate.UserAcceptedTermsVersions = new List<DataModels.UserAcceptedTermsVersion>()
                        {
                            new DataModels.UserAcceptedTermsVersion(currentTerms)
                        };
                    }
                }
                return new User(_context.CreateUser(userToCreate));
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to create user {user.Auth0UserId} - exception: {e.Message}");
                throw new Exception($"Failed to create user {user.Auth0UserId} - exception: {e.Message}");
            }
        }

        public User GetUser(int userId)
        {
            var user = _context.Users.Where((u => u.UserId == userId)).FirstOrDefault();
            return user != null ? new User(user) : null;
        }

        public List<User> GetUsers()
        {
            return _context.Users.Select(user => new User(user)).ToList();
        }

        public List<UserDetails> FindUsers(IEnumerable<string> auth0UserIds)
        {
			return _context.Users.Where(user => auth0UserIds.Contains(user.Auth0UserId)).Select(user => new UserDetails(user.Auth0UserId, user.DisplayName, user.EmailAddress)).ToList();
		}

        public Dictionary<string, IEnumerable<string>> FindRoles(IEnumerable<string> auth0UserIds, string host)
        {
	        var users = _context.GetUsers(auth0UserIds);
	        return users.ToDictionary(user => user.Auth0UserId, 
						user => user.UserRoles
							.Where(userRole => userRole.Role.Website.Host.Equals(host, StringComparison.OrdinalIgnoreCase))
							.Select(role => role.Role.Name));
        }

		// TODO: update user in identity provider if needed
		public User UpdateUser(int userId, User user)
        {
            try
            {
                var userToUpdate = _context.GetUser(userId);
                if (userToUpdate == null)
                    throw new Exception($"User not found {userId.ToString()}");

                userToUpdate.UpdateFromApiModel(user);
                _providerManagementService.UpdateUser(userToUpdate.Auth0UserId, userToUpdate);

                _context.SaveChanges();
                return new User(userToUpdate);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to update user {userId.ToString()} - exception: {e.InnerException.Message}");
                throw new Exception($"Failed to update user {userId.ToString()} - exception: {e.InnerException.Message}");
            }
        }

        public int DeleteUser(int userId) 
        {
            try
            {
                var userToDelete = _context.GetUser(userId);
                if (userToDelete == null)
                    return 0;

                _context.Users.RemoveRange(userToDelete);
                _providerManagementService.DeleteUser(userToDelete.Auth0UserId);

                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to delete user {userId.ToString()} - exception: {e.Message}");
                throw new Exception($"Failed to delete user {userId.ToString()} - exception: {e.Message}");
            }
        }
    }
}