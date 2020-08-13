using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public interface IOrganisationRolesService
    {
        OrganisationRole CreateOrganisationRole(OrganisationRole organisationRole);
        List<OrganisationRole> GetOrganisationRoles();
        OrganisationRole GetOrganisationRole(int organisationRoleId);
        int DeleteOrganisationRole(int organisationRoleId);
    }

    public class OrganisationRolesService : IOrganisationRolesService
    {
        private readonly IdentityContext _context;
        private readonly ILogger<OrganisationRolesService> _logger;
        public OrganisationRolesService(IdentityContext context, ILogger<OrganisationRolesService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public OrganisationRole CreateOrganisationRole(OrganisationRole organisationRole)
        {
            try
            {
                var organisationRoleToCreate = new DataModels.OrganisationRole();
                organisationRoleToCreate.UpdateFromApiModel(organisationRole);
                var createdRole = _context.OrganisationRoles.Add(organisationRoleToCreate);
                _context.SaveChanges();
                return new OrganisationRole(createdRole.Entity);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to create organisation role for {organisationRole.OrganisationId.ToString()} - exception: {e.Message}");
                throw new Exception($"Failed to create organisation role for {organisationRole.OrganisationId.ToString()} - exception: {e.Message}");
            }

        }

        public List<OrganisationRole> GetOrganisationRoles()
        {
            return _context.OrganisationRoles.Select(organisationRole => new OrganisationRole(organisationRole)).ToList();
        }

        public OrganisationRole GetOrganisationRole(int organisationRoleId)
        {
            var organisationRole = _context.OrganisationRoles.Where((o => o.OrganisationRoleId == organisationRoleId)).FirstOrDefault();
            return organisationRole != null ? new OrganisationRole(organisationRole) : null;
        }

        public int DeleteOrganisationRole(int organisationRoleId)
        {
            try
            {
                var organisationRoleToDelete = _context.OrganisationRoles.Find(organisationRoleId);
                if (organisationRoleToDelete == null)
                    return 0;
                _context.OrganisationRoles.RemoveRange(organisationRoleToDelete);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to delete organisation role {organisationRoleId.ToString()} - exception: {e.Message}");
                throw new Exception($"Failed to delete organisation role {organisationRoleId.ToString()} - exception: {e.Message}");
            }
        }
    }
}
