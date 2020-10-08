using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authorisation.WebAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using NICE.Identity.Authorisation.WebAPI.HealthChecks;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
    [Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
	[ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IUsersService _usersService;
		private readonly IProviderManagementService _providerManagementService;
		private readonly IDuplicateCheck _duplicateCheck;
		private readonly IUserSyncCheck _userSyncCheck;

		public AdminController(ILogger<AdminController> logger, IWebHostEnvironment environment, IUsersService usersService, IProviderManagementService providerManagementService,
			IDuplicateCheck duplicateCheck, IUserSyncCheck userSyncCheck)
        {
	        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
	        _environment = environment;
	        _usersService = usersService;
			_providerManagementService = providerManagementService;
			_duplicateCheck = duplicateCheck;
			_userSyncCheck = userSyncCheck;
        }

        /// <summary>
        /// delete user with id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
        public async Task<IActionResult> DeleteAllUsers()
        {
            _logger.LogInformation($"Delete all users called by {User.Identity.Name}");

	        if (_environment.IsProduction())
	        {
		        _logger.LogError($"Delete all users called on production. returning a 500");
                return StatusCode(500, new ProblemDetails { Status = 500, Title = $"Deleting all users is not allowed on production" });
            }
	        try
	        {
		        _logger.LogWarning($"Deleting all users called.");
                var recordCountUpdated = await _usersService.DeleteAllUsers();
		        return Ok(new {recordCountUpdated });
	        }
	        catch (Exception e)
	        {
		        _logger.LogError(e,$"Deleting all users failed with error {e.ToString()}");
                return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
	        }
        }

		/// <summary>
		/// Gets the auth0 management api token. for use by the front-end dashboard page, where it should be cached.
		/// </summary>
		/// <returns></returns>
		[HttpGet("getmanagementapitoken")]
		[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Produces("application/json")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
		public async Task<IActionResult> GetManagementAPIToken()
		{
			try
			{
				return Ok(await _providerManagementService.GetAccessTokenForManagementAPI());
			}
			catch (Exception e)
			{
				return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
			}
		}

		/// <summary>
		/// The health check api reports whether there's duplicate users or not.
		/// this _authenticated_ (user admin) endpoint actually provides the duplicate user's email addresses.
		/// </summary>
		/// <returns></returns>
		[HttpGet("getduplicateusers")]
		[ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Produces("application/json")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
		public IActionResult GetDuplicateUsers()
		{
			try
			{
				return Ok(_duplicateCheck.GetDuplicateUsers());
			}
			catch (Exception e)
			{
				return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
			}
		}

		[HttpGet("getusersyncdata")]
		[ProducesResponseType(typeof(UserSync), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Produces("application/json")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
		public async Task<IActionResult> GetUserSyncData()
		{
			try
			{
				return Ok(await _userSyncCheck.GetUserSyncData());
			}
			catch (Exception e)
			{
				return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
			}
		}
	}
}