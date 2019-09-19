using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Repositories;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public interface IUsersService
    {
        User CreateUser(User user);
        User GetUser(int userId);
        List<User> GetUsers();
        int UpdateUser(User user);
        int DeleteUser(int userId);
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
                var userToCreate = new DataModels.User();
                userToCreate.UpdateFromApiModel(user);
                if (user.AcceptedTerms)
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
                _logger.LogError($"Failed to add user {user.Auth0UserId} - exception: {e.Message}");
                throw new Exception($"Failed to add user {user.Auth0UserId} - exception: {e.Message}");
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

        public int UpdateUser(User user)
        {
            try
            {
                var userToUpdate = _context.GetUser(user.UserId);
                if (userToUpdate == null)
                    return 0;

                userToUpdate.UpdateFromApiModel(user);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to update user {user.UserId.ToString()} - exception: {e.Message}");
                throw new Exception($"Failed to update user {user.UserId.ToString()} - exception: {e.Message}");
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