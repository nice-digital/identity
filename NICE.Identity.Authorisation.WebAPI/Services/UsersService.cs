using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using IdentityContext = NICE.Identity.Authorisation.WebAPI.Repositories.IdentityContext;
using User = NICE.Identity.Authorisation.WebAPI.ApiModels.User;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IUsersService
    {
        User CreateUser(User user);
        User UpdateUser(User user);
        List<User> GetUsers();
        int DeleteUser(int userId);
        User GetUser(int userId);
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

        public User CreateUser(User user)
        {
            try
            {
                var userEntity = MapUserToDomainModel(user);
                return new User(_context.AddUser(userEntity));
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to add user {user.Auth0UserId} - exception: {e.Message}");
                throw new Exception($"Failed to add user {user.Auth0UserId} - exception: {e.Message}");
            }
        }
        
        public User UpdateUser(User user)
        {
            try
            {
                var userEntity = MapUserToDomainModel(user);
                return new User(_context.UpdateUser(userEntity));
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to add user - " +
                                 $"exception: '{e.Message}' userId: '{user.Auth0UserId}'");
                throw new Exception("Failed to add user");
            }
        }

        public List<User> GetUsers()
        {
            return _context.Users.Select(user => new User(user)).ToList();
        }

        public int DeleteUser(int userId) 
        {
            return _context.DeleteUser(userId);
        }

        public User GetUser(int userId)
        {
            var user = _context.Users.Where((u => u.UserId == userId)).FirstOrDefault();
            return user != null ? new User(user) : null;
        }

        private DataModels.User MapUserToDomainModel(ApiModels.User user)
        {           
            var userEntity = new DataModels.User
            {
                //AcceptedTerms = user.AcceptedTerms,
                UserId = user.UserId,
                Auth0UserId = user.Auth0UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AllowContactMe = user.AllowContactMe,
                InitialRegistrationDate = user.InitialRegistrationDate,
                LastLoggedInDate = user.LastLoggedInDate,
                HasVerifiedEmailAddress = user.HasVerifiedEmailAddress,
                EmailAddress = user.EmailAddress,
                IsLockedOut = user.IsLockedOut,
                IsStaffMember = user.IsStaffMember,
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