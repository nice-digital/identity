using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Authentication.Sdk;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
    [ApiController]
    public class OrganisationsController : ControllerBase
    {
        private readonly ILogger<OrganisationsController> _logger;
        private readonly IOrganisationsService _organisationService;

        public OrganisationsController(IOrganisationsService organisationService, ILogger<OrganisationsController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _organisationService = organisationService ?? throw new ArgumentNullException(nameof(organisationService));
        }

        /// <summary>
        /// create organisation
        /// </summary>
        /// <param name="organisation"></param>
        /// <returns></returns>
        [HttpPost("")]
        [ProducesResponseType(typeof(Organisation), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult CreateOrganisation(Organisation organisation)
        {
            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
                _logger.LogError($"Invalid model for create organisation", modelStateJson);
                return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid model for create organisation" });
            }

            try
            {
                var createdOrganisation = _organisationService.CreateOrganisation(organisation);
                return Created($"/organsation/{createdOrganisation.OrganisationId.ToString()}", createdOrganisation);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = e.Message, Detail = e.ToString() });
            }
        }

        /// <summary>
        /// get list of all organisation
        /// </summary>
        /// <returns>returns all organisations</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(List<Organisation>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetOrganisations()
        {
            try
            {
                return Ok(_organisationService.GetOrganisations());
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
            }
        }

        /// <summary>
        /// get organisation with id
        /// api/organisation/1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Organisation), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetOrganisation(int id)
        {
            try
            {
                var organisation = _organisationService.GetOrganisation(id);
                if (organisation != null)
                {
                    return Ok(organisation);
                }
                return NotFound(new ProblemDetails { Status = 404, Title = "Organisation not found" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
            }
        }

        /// <summary>
        /// update organisation with id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="organisation"></param>
        /// <returns></returns>
        [HttpPatch("{id}", Name = "UpdateOrganisationPartial")]
        [HttpPut("{id}", Name = "UpdateOrganisation")]
        [ProducesResponseType(typeof(Organisation), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult UpdateOrganisation(int id, Organisation organisation)
        {
            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
                _logger.LogError($"Invalid model for update organisation {modelStateJson}");
                return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid model for update organisation" });
            }

            try
            {
                return Ok(_organisationService.UpdateOrganisation(id, organisation));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
            }
        }

        /// <summary>
        /// delete organsation with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult DeleteOrganisation(int id)
        {
            try
            {
                _organisationService.DeleteOrganisation(id);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
            }
        }

        /// <summary>
        /// get list of all users given in the nameIdentifiers parameter
        /// </summary>
        /// <param name="nameIdentifiers">this was the auth0UserId, now it's the "Name identifier"</param>
        /// <returns></returns>
        [HttpPost(Constants.AuthorisationURLs.GetOrganisationsRoute)]
        [ProducesResponseType(typeof(List<Authentication.Sdk.Domain.Organisation>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration + "," + Policies.API.RolesWithAccessToUserProfilesPlaceholder)]
        public IActionResult GetOrganisationsByIds([FromBody] IEnumerable<int> organisationIds)
        {
	        try
	        {
		        var organisations = _organisationService.GetOrganisationsByOrganisationIds(organisationIds);
		        return Ok(organisations);
	        }
	        catch (Exception e)
	        {
		        return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
	        }
        }
    }
}
