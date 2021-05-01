using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authentication.Sdk.Extensions;
using NICE.Identity.Authorisation.WebAPI.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using User = NICE.Identity.Authorisation.WebAPI.ApiModels.User;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] //can't add the UserAdministration policy at this level as the find users and find roles actions don't need it.
	[ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUsersService _usersService;

        public UserProfileController(IUsersService usersService, ILogger<UsersController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }

        private string GetNameIdentifierFromUser()
        {
	        var claimsPrincipal = HttpContext?.User; //todo: switch to using httpcontextaccessor..

	        return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        }
        
		/// <summary>
		/// get list of all users
		/// </summary>
		/// <returns></returns>
		[HttpGet("")]
		[ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Produces("application/json")]
		public IActionResult GetOwnUserProfile()
		{
			try
			{
				var nameIdentifier = GetNameIdentifierFromUser();
				if (nameIdentifier == null)
				{
					return StatusCode(500, new ProblemDetails { Status = 500, Title = $"Unable to get name identifier when retrieving own profile" });
				}
				return Ok(_usersService.GetUser(nameIdentifier));
			}
			catch (Exception e)
			{
				return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
			}
		}

		/// <summary>
		/// get list of all users
		/// </summary>
		/// <returns></returns>
		[HttpPost("")]
		[ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Produces("application/json")]
		public async Task<IActionResult> GetOwnUserProfile(User user)
		{
			try
			{
				var nameIdentifier = GetNameIdentifierFromUser();

				if (string.IsNullOrEmpty(nameIdentifier) || !nameIdentifier.Equals(user.NameIdentifier))
				{
					return StatusCode(500, new ProblemDetails { Status = 500, Title = $"Invalid user" });
				}

				var userIdToUpdate = _usersService.GetUser(nameIdentifier)?.UserId;
				if (!userIdToUpdate.HasValue)
				{
					return StatusCode(500, new ProblemDetails { Status = 500, Title = $"Unable to get user when updating own profile" });
				}

				var updatedUser = await _usersService.UpdateUser(userIdToUpdate.Value, user); //todo: more security here.
				return Ok(updatedUser);
			}
			catch (Exception e)
			{
				return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
			}
		}
    }
}