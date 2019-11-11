using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NICE.Identity.Authentication.Sdk.Domain;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
		public IActionResult GetUsers()
		{
			try
			{
				return Ok(_usersService.GetUsers());
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
		/// get list of all users given in the auth0UserIds parameter
		/// </summary>
		/// <param name="nameIdentifiers">this is the auth0UserId aka the "Name identifier"</param>
		/// <returns></returns>
		[HttpPost("findusers")]
		[ProducesResponseType(typeof(List<UserDetails>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Produces("application/json")]
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
		/// returns a dictionary of all the users supplied along with a list of their available roles.
		/// </summary>
		/// <param name="nameIdentifiers">this is the auth0UserId aka the "Name identifier"</param>
		/// <returns></returns>
		[HttpPost("findroles/{host}")]
		[ProducesResponseType(typeof(Dictionary<string, IEnumerable<string>>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Produces("application/json")]
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

		// TODO: custom model validation, checking incoming properties
		/// <summary>
		/// update user with id
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		[HttpPatch("{userId}")]
        [HttpPut("{userId}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult UpdateUser([FromRoute] int userId, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
                _logger.LogError($"Invalid model for update user {modelStateJson}");
                return BadRequest(new ProblemDetails {Status = 400, Title = "Invalid model for update user"});
            }

            if (userId != user.UserId)
            {
                return BadRequest(new ProblemDetails {Status = 400, Title = "UserId does not match"});
            }

            try
            {
                return Ok(_usersService.UpdateUser(userId, user));
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
        // TODO: Tidy up return for delete user
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult DeleteUser(int userId)
        {
            try
            {
                _usersService.DeleteUser(userId);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = $"{e.Message}"});
            }
        }
    }
}