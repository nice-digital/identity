using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Services;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganisationController : ControllerBase
    {
        private readonly ILogger<OrganisationController> _logger;
        private readonly IOrganisationService _organisationService;

        public OrganisationController(IOrganisationService organisationService, ILogger<OrganisationController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _organisationService = organisationService ?? throw new ArgumentNullException(nameof(organisationService));
        }

        /// <summary>
        /// create organisation
        /// </summary>
        /// <param name="organisation"></param>
        /// <returns></returns>
        //[HttpPost("")]
        //[ProducesResponseType(typeof(Organisation), StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status409Conflict)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Consumes("application/json")]
        //[Produces("application/json")]
        //public IActionResult CreateRole(Organisation organisation)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var serializableModelState = new SerializableError(ModelState);
        //        var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
        //        _logger.LogError($"Invalid model for create role", modelStateJson);
        //        return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid model for create role" });
        //    }

        //    try
        //    {
        //        var createdRole = _rolesService.CreateRole(role);
        //        return Created($"/role/{createdRole.RoleId.ToString()}", createdRole);
        //    }
        //    catch (Exception e)
        //    {
        //        return StatusCode(500, new ProblemDetails { Status = 500, Title = e.Message, Detail = e.InnerException?.Message });
        //    }
        //}

        /// <summary>
        /// get list of all organisation
        /// </summary>
        /// <returns>returns all organisations</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(List<Organisation>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetRoles()
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
        [ProducesResponseType(typeof(Role), StatusCodes.Status200OK)]
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
    }
}
