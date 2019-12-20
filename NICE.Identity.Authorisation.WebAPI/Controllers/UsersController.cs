using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NICE.Identity.Authorisation.WebAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NICE.Identity.Authentication.Sdk;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Configuration;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using User = NICE.Identity.Authorisation.WebAPI.ApiModels.User;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
    [Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] //can't add the UserAdministration policy at this level as the find users and find roles actions don't need it.
	[ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService, ILogger<UsersController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }

        /// <summary>
        /// create user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        // TODO: Conflict/409 if user already exists instead of 500
        [HttpPost("")]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
		public IActionResult CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
                _logger.LogError($"Invalid model for create user", modelStateJson);
                return BadRequest(new ProblemDetails {Status = 400, Title = "Invalid model for create user"});
            }

            try
            {
	            user.IsInAuthenticationProvider = true; //currently this endpoint is only hit by the authentication provider, so setting here makes sense. 
                var createdUser = _usersService.CreateUser(user);
                return Created($"/user/{createdUser.UserId.ToString()}",createdUser);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = e.Message, Detail = e.InnerException?.Message});
            }
        }

		/// <summary>
		/// get list of all users
		/// </summary>
		/// <returns></returns>
		[HttpGet("")]
		[ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Produces("application/json")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
        public IActionResult GetUsers([FromQuery(Name = "q")] string filter)
		{
			try
			{
				return Ok(_usersService.GetUsers(filter));
			}
			catch (Exception e)
			{
				return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
			}
		}

		/// <summary>
		/// get user with id
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[HttpGet("{userId:int}")]
		[ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Produces("application/json")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
		public IActionResult GetUser(int userId)
		{
			try
			{
				var user = _usersService.GetUser(userId);
				if (user != null)
				{
					return Ok(user);
				}
				return NotFound(new ProblemDetails { Status = 404, Title = "User not found" });
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
		[HttpPost(Constants.AuthorisationURLs.FindUsersRoute)]
		[ProducesResponseType(typeof(List<UserDetails>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Produces("application/json")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration + "," + Policies.API.RolesWithAccessToUserProfilesPlaceholder)]
		public IActionResult FindUsers([FromBody] IEnumerable<string> nameIdentifiers)
        {
			try
			{
				return Ok(_usersService.FindUsers(nameIdentifiers.Distinct()));
			}
			catch (Exception e)
			{
				return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
			}
		}

        /// <summary>
        /// FindRoles - gets a list of the all the roles for the given users for the given host
        /// </summary>
        /// <param name="nameIdentifiers"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        [HttpPost( Constants.AuthorisationURLs.FindRolesRoute + "{host}")]
        [ProducesResponseType(typeof(Dictionary<string, IEnumerable<string>>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Produces("application/json")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration + "," + Policies.API.RolesWithAccessToUserProfilesPlaceholder)]
		public IActionResult FindRoles([FromBody] IEnumerable<string> nameIdentifiers, string host)
		{
			try
			{
				return Ok(_usersService.FindRoles(nameIdentifiers.Distinct(), host));
			}
			catch (Exception e)
			{
				return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
			}
		}

        /// <summary>
        /// update user with id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPatch("{userId}", Name = "UpdateUserPartial")]
        [HttpPut("{userId}", Name = "UpdateUser")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
		public async Task<IActionResult> UpdateUser([FromRoute] int userId, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
                _logger.LogError($"Invalid model for update user {modelStateJson}");
                return BadRequest(new ProblemDetails {Status = 400, Title = "Invalid model for update user"});
            }

            try
            {
                return Ok(await _usersService.UpdateUser(userId, user));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = $"{e.Message}"});
            }
        }

        /// <summary>
        /// delete user with id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
		public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                await _usersService.DeleteUser(userId);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = $"{e.Message}"});
            }
        }

        [HttpPost("import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
		public IActionResult ImportUsers([FromBody] IList<ImportUser> usersToImport)
        {
	        if (!ModelState.IsValid)
	        {
		        var serializableModelState = new SerializableError(ModelState);
		        var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
		        _logger.LogError($"Invalid model for create user", modelStateJson);
		        return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid model for create user" });
	        }

	        try
	        {
		        _usersService.ImportUsers(usersToImport);
		        return Ok();
	        }
	        catch (Exception e)
	        {
		        return StatusCode(500, new ProblemDetails { Status = 500, Title = e.Message, Detail = e.InnerException?.Message });
	        }
        }

        /// <summary>
        /// gets the user roles for the specified user and website
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="websiteId"></param>
        /// <returns></returns>
        [HttpGet("{userId:int}/rolesbywebsite/{websiteId:int}")]
        [ProducesResponseType(typeof(UserRolesByWebsite), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
		public IActionResult GetRolesForUserByWebsite(int userId, int websiteId)
        {
            try
            {
                var userRolesByWebsite = _usersService.GetRolesForUserByWebsite(userId, websiteId);
                if (userRolesByWebsite != null)
                {
                    return Ok(userRolesByWebsite);
                }
                return NotFound(new ProblemDetails { Status = 404, Title = "User not found" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
            }
        }

        
        
        /// <summary>
        /// updates the user roles for the specified user and website
        /// </summary>
        /// <description>
        /// updates the user roles for the specified user and website.
        /// if the user has the role and HasRole is false remove the role
        /// if the user does not have the role and and HasRole is true add the role
        /// </description>
        /// <param name="userId"></param>
        /// <param name="websiteId"></param>
        /// <param name="userRolesByWebsite"></param>
        /// <returns></returns>
        [HttpPut("{userId:int}/rolesbywebsite/{websiteId:int}", Name = "UpdateRolesForUserByWebsite")]
        [HttpPatch("{userId:int}/rolesbywebsite/{websiteId:int}", Name = "UpdateRolesForUserByWebsitePartial")]
        [ProducesResponseType(typeof(UserRolesByWebsite), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
		public IActionResult UpdateRolesForUserByWebsite([FromRoute] int userId, [FromRoute] int websiteId, 
            [FromBody] UserRolesByWebsite userRolesByWebsite)
        {
            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
                _logger.LogError($"Invalid model for update user {modelStateJson}");
                return BadRequest(new ProblemDetails {Status = 400, Title = "Invalid model for update user"});
            }

            try
            {
                return Ok(_usersService.UpdateRolesForUserByWebsite(userId, websiteId, userRolesByWebsite));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = $"{e.Message}"});
            }
        }
        
        /// <summary>
        /// gets the user roles for the specified user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId:int}/roles")]
        [ProducesResponseType(typeof(List<ApiModels.UserRole>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
		public IActionResult GetRolesForUser(int userId)
        {
            try
            {
                var userRoles = _usersService.GetRolesForUser(userId);
                if (userRoles != null)
                {
                    return Ok(userRoles);
                }
                return NotFound(new ProblemDetails { Status = 404, Title = "User not found" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
            }
        }

        /// <summary>
        /// updates the user roles for the specified user
        /// </summary>
        /// <description>
        /// updates the user roles for the specified user.
        /// if the user has the role it updates it.
        /// if the user does not have the role it adds it
        /// </description>
        /// <param name="userId"></param>
        /// <param name="userRoles"></param>
        /// <returns></returns>
        [HttpPut("{userId:int}/roles", Name = "UpdateRolesForUser")]
        [ProducesResponseType(typeof(List<ApiModels.UserRole>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
		public IActionResult UpdateRolesForUser([FromRoute] int userId, [FromBody] List<ApiModels.UserRole> userRoles)
        {
            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
                _logger.LogError($"Invalid model for update user {modelStateJson}");
                return BadRequest(new ProblemDetails {Status = 400, Title = "Invalid model for update user"});
            }

            try
            {
                return Ok(_usersService.UpdateRolesForUser(userId, userRoles));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = $"{e.Message}"});
            }
        }
    }
}