using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Repositories;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public interface IUserRolesService
    {
        UserRole CreateUserRole(UserRole userRole);
        List<UserRole> GetUserRoles();
        UserRole GetUserRole(int userRoleId);
        int DeleteUserRole(int userRoleId);
    }

    public class UserRolesService : IUserRolesService
    {
        private readonly IdentityContext _context;
        private readonly ILogger<UserRolesService> _logger;
        public UserRolesService(IdentityContext context, ILogger<UserRolesService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public UserRole CreateUserRole(UserRole userRole)
        {
            try
            {
                var userRoleToCreate = new DataModels.UserRole();
                userRoleToCreate.UpdateFromApiModel(userRole);
                var createdRole = _context.UserRoles.Add(userRoleToCreate);
                _context.SaveChanges();
                return new UserRole(createdRole.Entity);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to create user role for {userRole.UserId.ToString()} - exception: {e.Message}");
                throw new Exception($"Failed to create user role for {userRole.UserId.ToString()} - exception: {e.Message}");
            }
        }

        public List<UserRole> GetUserRoles()
        {
            return _context.UserRoles.Select(userRole => new UserRole(userRole)).ToList();
        }

        public UserRole GetUserRole(int userRoleId)
        {
            var userRole = _context.UserRoles.Where((u => u.UserRoleId == userRoleId)).FirstOrDefault();
            return userRole != null ? new UserRole(userRole) : null;
        }

        public int DeleteUserRole(int userRoleId)
        {
            try
            {
                var userRoleToDelete = _context.UserRoles.Find(userRoleId);
                if (userRoleToDelete == null)
                    return 0;
                _context.UserRoles.RemoveRange(userRoleToDelete);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to delete user role {userRoleId.ToString()} - exception: {e.Message}");
                throw new Exception($"Failed to delete user role {userRoleId.ToString()} - exception: {e.Message}");
            }
        }
    }
}
