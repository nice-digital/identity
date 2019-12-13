using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Repositories;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public interface IRolesService
    {
        Role CreateRole(Role role);
        List<Role> GetRoles();
        Role GetRole(int roleId);
        Role UpdateRole(int roleId, Role role);
        int DeleteRole(int roleId);
    }

    public class RolesService : IRolesService
    {
        private readonly IdentityContext _context;
        private readonly ILogger<RolesService> _logger;

        public RolesService(IdentityContext context, ILogger<RolesService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Role CreateRole(Role role)
        {
            try
            {
                var roleToCreate = new DataModels.Role();
                roleToCreate.UpdateFromApiModel(role);
                var createdRole = _context.Roles.Add(roleToCreate);
                _context.SaveChanges();
                return new Role(createdRole.Entity);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to create role {role.Name} - exception: {e.Message}");
                throw new Exception($"Failed to create role {role.Name} - exception: {e.Message}");
            }
        }

        public List<Role> GetRoles()
        {
            return _context.Roles.Select(role => new Role(role)).ToList();
        }

        public Role GetRole(int roleId)
        {
            var role = _context.Roles.Where((r => r.RoleId == roleId)).FirstOrDefault();
            return role != null ? new Role(role) : null;
        }

        public Role UpdateRole(int roleId, Role role)
        {
            try
            {
                var roleToUpdate = _context.Roles.Find(roleId);
                if (roleToUpdate == null)
                    throw new Exception($"Role not found {roleId.ToString()}");

                roleToUpdate.UpdateFromApiModel(role);
                _context.SaveChanges();
                return new Role(roleToUpdate);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to update role {roleId.ToString()} - exception: {e.Message}");
                throw new Exception($"Failed to update role {roleId.ToString()} - exception: {e.Message}");
            }
        }

        public int DeleteRole(int roleId)
        {
            try
            {
                var roleToDelete = _context.Roles.Find(roleId);
                if (roleToDelete == null)
                    return 0;
                _context.Roles.RemoveRange(roleToDelete);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to delete role {roleId.ToString()} - exception: {e.Message}");
                throw new Exception($"Failed to delete role {roleId.ToString()} - exception: {e.Message}");
            }
        }
    }
}