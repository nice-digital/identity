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

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
    [ApiController]
    public class OrganisationRolesController : ControllerBase
    {
        private readonly IOrganisationRolesService _organisationRolesService;
        private readonly ILogger<OrganisationRolesController> _logger;

        public OrganisationRolesController(IOrganisationRolesService organisationRolesService, ILogger<OrganisationRolesController> logger)
        {
            _organisationRolesService = organisationRolesService ?? throw new ArgumentNullException(nameof(organisationRolesService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// create organisation role
        /// </summary>
        /// <param name="organisationRole"></param>
        /// <returns></returns>
        [HttpPost("")]
        [ProducesResponseType(typeof(OrganisationRole), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult CreateOrganisationRole(OrganisationRole organisationRole)
        {
            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
                _logger.LogError($"Invalid model for create organisation role", modelStateJson);
                return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid model for create organisation role" });
            }

            try
            {
                var createdOrganisationRole = _organisationRolesService.CreateOrganisationRole(organisationRole);
                return Created($"/organisationrole/{createdOrganisationRole.OrganisationRoleId.ToString()}", createdOrganisationRole);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = e.Message, Detail = e.ToString() });
            }
        }

        /// <summary>
        /// get list of all organisation roles
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(List<OrganisationRole>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetOrganisationRoles()
        {
            try
            {
                return Ok(_organisationRolesService.GetOrganisationRoles());
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
            }
        }

        /// <summary>
        /// get organisation role with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrganisationRole), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetOrganisationRole(int id)
        {
            try
            {
                var organisationRole = _organisationRolesService.GetOrganisationRole(id);
                if (organisationRole != null)
                {
                    return Ok(organisationRole);
                }
                return NotFound(new ProblemDetails { Status = 404, Title = "Organisation role not found" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
            }
        }

        /// <summary>
        /// delete organisation role with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult DeleteOrganisationRole(int id)
        {
            try
            {
                _organisationRolesService.DeleteOrganisationRole(id);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
            }
        }
    }
}
