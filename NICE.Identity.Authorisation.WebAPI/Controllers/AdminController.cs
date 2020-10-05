using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authorisation.WebAPI.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

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
		public AdminController(ILogger<AdminController> logger, IWebHostEnvironment environment, IUsersService usersService, IProviderManagementService providerManagementService)
        {
	        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
	        _environment = environment;
	        _usersService = usersService;
			_providerManagementService = providerManagementService;
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
		/// get user with id
		/// </summary>
		/// <param name="userId"></param>
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
	}
}