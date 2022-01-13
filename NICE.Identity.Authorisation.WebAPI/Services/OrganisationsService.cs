using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public interface IOrganisationsService
    {
        List<Organisation> GetOrganisations(string filter);
        Organisation GetOrganisation(int organisationId);
        Organisation CreateOrganisation(Organisation organisation);
        Organisation UpdateOrganisation(int organisationId, Organisation organisation);
        int DeleteOrganisation(int organisationId);

        IEnumerable<Authentication.Sdk.Domain.Organisation> GetOrganisationsByOrganisationIds(IEnumerable<int> organisationIds);
    }

    public class OrganisationsService : IOrganisationsService
    {
        private readonly IdentityContext _context;
        private readonly ILogger<OrganisationsService> _logger;

        public OrganisationsService(IdentityContext context, ILogger<OrganisationsService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Organisation CreateOrganisation(Organisation organisation)
        {
            try
            {
                var organisationToCreate = new DataModels.Organisation();
                organisationToCreate.UpdateFromApiModel(organisation, true);
                var createdOrganisation = _context.Organisations.Add(organisationToCreate);
                _context.SaveChanges();
                return new Organisation(createdOrganisation.Entity);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to create organisation {organisation.Name} - exception: {e.Message}");
                throw new Exception($"Failed to create organisation {organisation.Name} - exception: {e.Message}");
            }
        }

        public List<Organisation> GetOrganisations(string filter = null)
        {
            return _context.FindOrganisations(filter)
                    .Select(organisation => new Organisation(organisation))
                    .ToList();
        }

        public Organisation GetOrganisation(int organisationId)
        {
            var organisation = _context.Organisations.Where(organisation => organisation.OrganisationId == organisationId).FirstOrDefault();
            return organisation != null ? new Organisation(organisation) : null;
        }

        public Organisation UpdateOrganisation(int organisationId, Organisation organisation)
        {
            try
            {
                var organisationToUpdate = _context.Organisations.Find(organisationId);
                if (organisationToUpdate == null)
                    throw new Exception($"Organisation not found {organisationId.ToString()}");

                organisationToUpdate.UpdateFromApiModel(organisation, false);
                _context.SaveChanges();
                return new Organisation(organisationToUpdate);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to update organisation {organisationId.ToString()} - exception: {e.Message}");
                throw new Exception($"Failed to update organisation {organisationId.ToString()} - exception: {e.Message}");
            }
        }

        public int DeleteOrganisation(int organisationId)
        {
            try
            {
                var organisationToDelete = _context.Organisations.Find(organisationId);
                if (organisationToDelete == null)
                    return 0;
                _context.Organisations.RemoveRange(organisationToDelete);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to delete organisation {organisationId.ToString()} - exception: {e.Message}");
                throw new Exception($"Failed to delete organisation {organisationId.ToString()} - exception: {e.Message}");
            }
        }

        public IEnumerable<Authentication.Sdk.Domain.Organisation> GetOrganisationsByOrganisationIds(IEnumerable<int> organisationIds)
        {
	        var matchingOrganisationsInDatabase =  _context.Organisations.Where(org => organisationIds.Contains(org.OrganisationId)).ToList();

	        return matchingOrganisationsInDatabase.Select(org => new Authentication.Sdk.Domain.Organisation(org.OrganisationId, org.Name, false));
        }
    }
}
