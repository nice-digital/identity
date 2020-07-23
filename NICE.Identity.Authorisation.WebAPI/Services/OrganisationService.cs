using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.APIModels;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public interface IOrganisationService
    {

    }

    public class OrganisationService : IOrganisationService
    {
        private readonly IdentityContext _context;
        private readonly ILogger<OrganisationService> _logger;

        public OrganisationService(IdentityContext context, ILogger<OrganisationService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public List<Organisation> GetOrganisations()
        {
            return new List<Organisation>();
        }

        public Organisation CreateOrganisation(Organisation organisation)
        {
            try
            {
                var organisationToCreate = new DataModels.Organisation();
                organisationToCreate.UpdateFromApiModel(organisation);
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
    }
}
