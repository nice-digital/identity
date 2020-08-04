using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Services;
using System;
using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
	[ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ILogger<RolesController> _logger;
        private readonly IRolesService _rolesService;

        public RolesController(IRolesService rolesService, ILogger<RolesController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _rolesService = rolesService ?? throw new ArgumentNullException(nameof(rolesService));
        }
        
        /// <summary>
        /// create role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost("")]
        [ProducesResponseType(typeof(Role), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
		public IActionResult CreateRole(Role role)
        {
            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
                _logger.LogError($"Invalid model for create role", modelStateJson);
                return BadRequest(new ProblemDetails {Status = 400, Title = "Invalid model for create role"});
            }

            try
            {
                var createdRole = _rolesService.CreateRole(role);
                return Created($"/role/{createdRole.RoleId.ToString()}", createdRole);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = e.Message, Detail = e.ToString() });
            }
        }
        
        /// <summary>
        /// get list of all roles
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(List<Role>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
		public IActionResult GetRoles()
        {
            try
            {
                return Ok(_rolesService.GetRoles());
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = $"{e.Message}"});
            }
        }
        
        /// <summary>
        /// get role with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Role), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
		public IActionResult GetRole(int id)
        {
            try
            {
                var role = _rolesService.GetRole(id);
                if (role != null)
                {
                    return Ok(role);
                }
                return NotFound(new ProblemDetails {Status = 404, Title = "Role not found"});
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = $"{e.Message}"});
            }
        }
        
        /// <summary>
        /// update role with id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPatch("{id}", Name = "UpdateRolePartial")]
        [HttpPut("{id}", Name = "UpdateRole")]
        [ProducesResponseType(typeof(Role), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
		public IActionResult UpdateRole(int id, Role role)
        {
            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
                _logger.LogError($"Invalid model for update role {modelStateJson}");
                return BadRequest(new ProblemDetails {Status = 400, Title = "Invalid model for update role"});
            }

            try
            {
                return Ok(_rolesService.UpdateRole(id, role));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = $"{e.Message}"});
            }
        }

        /// <summary>
        /// delete role with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult DeleteRole(int id)
        {
            try
            {
                _rolesService.DeleteRole(id);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = $"{e.Message}"});
            }
        }
    }
}