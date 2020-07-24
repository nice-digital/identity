using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public interface IOrganisationRolesService
    {
        OrganisationRole CreateOrganisationRole(OrganisationRole organisationRole);
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
    }

}
