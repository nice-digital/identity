using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.ApiModels.Requests;
using NICE.Identity.Authorisation.WebAPI.APIModels.Responses;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using IdentityContext = NICE.Identity.Authorisation.WebAPI.Repositories.IdentityContext;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IUsersService
    {
        UserInList CreateUser(CreateUser user);
        UserInList UpdateUser(CreateUser user);
        List<UserInList> GetUsers();
        void DeleteUser(int userId);
        UserInList GetUser(int userId);
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

        public UserInList CreateUser(CreateUser user)
        {
            try
            {
                var userEntity = MapUserToDomainModel(user);
                return new UserInList(_context.AddUser(userEntity));
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to add user {user.Auth0UserId} - exception: {e.Message}");
                throw new Exception($"Failed to add user {user.Auth0UserId} - exception: {e.Message}");
            }
        }
        
        public UserInList UpdateUser(CreateUser user)
        {
            try
            {
                var userEntity = MapUserToDomainModel(user);
                return new UserInList(_context.UpdateUser(userEntity));
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to add user - " +
                                 $"exception: '{e.Message}' userId: '{user.Auth0UserId}'");
                throw new Exception("Failed to add user");
            }
        }

        public List<UserInList> GetUsers()
        {
            return _context.Users.Select(user => new UserInList(user)).ToList();
        }

        public void DeleteUser(int userId) 
        {
            _context.DeleteUser(userId);
        }

        public UserInList GetUser(int userId)
        {
            var user = _context.Users.Where((u => u.UserId == userId)).FirstOrDefault();
            return user != null ? new UserInList(user) : null;
        }

        private DataModels.User MapUserToDomainModel(CreateUser user)
        {           
            var userEntity = new User
            {
                //AcceptedTerms = user.AcceptedTerms,
                //TODO: AllowContactMe = user.AllowContactMe,
                Auth0UserId = user.Auth0UserId,
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