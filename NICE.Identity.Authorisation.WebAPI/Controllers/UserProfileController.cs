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
		/// gets own profile details
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
		/// updates user details
		/// </summary>
		/// <returns></returns>
		[HttpPost("")]
		[ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Produces("application/json")]
		public async Task<IActionResult> GetOwnUserProfile(string nameIdentifier, string firstName, string lastName, string emailAddress)
		{
			try
			{
				if (string.IsNullOrEmpty(nameIdentifier))
					return StatusCode(500, new ProblemDetails { Status = 500, Title = $"Invalid identifier" });

				if (string.IsNullOrEmpty(firstName))
					return StatusCode(500, new ProblemDetails { Status = 500, Title = $"Invalid firstName" });

				if (string.IsNullOrEmpty(lastName))
					return StatusCode(500, new ProblemDetails { Status = 500, Title = $"Invalid lastName" });

				if (string.IsNullOrEmpty(emailAddress))
					return StatusCode(500, new ProblemDetails { Status = 500, Title = $"Invalid emailAddress" });


				var nameIdentifierFromToken = GetNameIdentifierFromUser();

				if (string.IsNullOrEmpty(nameIdentifierFromToken) || !nameIdentifier.Equals(nameIdentifierFromToken, StringComparison.OrdinalIgnoreCase))
				{
					return StatusCode(500, new ProblemDetails { Status = 500, Title = $"Invalid user" });
				}

				var userToUpdate = _usersService.GetUser(nameIdentifier);
				if (userToUpdate == null)
				{
					return StatusCode(500, new ProblemDetails { Status = 500, Title = $"Unable to get user when updating own profile" });
				}

				userToUpdate.FirstName = firstName;
				userToUpdate.LastName = lastName;
				userToUpdate.EmailAddress = emailAddress;

				var updatedUser = await _usersService.UpdateUser(userToUpdate.UserId.Value, userToUpdate); 
				return Ok(updatedUser);
			}
			catch (Exception e)
			{
				return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
			}
		}
    }
}