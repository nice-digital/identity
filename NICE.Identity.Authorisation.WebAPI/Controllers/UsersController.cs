using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NICE.Identity.Authorisation.WebAPI.ApiModels.Responses;
using NICE.Identity.Authorisation.WebAPI.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using CreateUser = NICE.Identity.Authorisation.WebAPI.ApiModels.Requests.CreateUser;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
	[Route("api/[controller]")]
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

		// POST api/users
		//[AuthoriseWithApiKey]
	    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] //, Policy = "administration")]
		[HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateUser user)
        {
            if (!ModelState.IsValid)
            {
	            var serializableModelState = new SerializableError(ModelState);
	            var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
	            _logger.LogError($"Invalid Model State at UsersController.Put for user id: {user.Auth0UserId}", modelStateJson);

				return BadRequest("Request failed validation");
            }

            try
            {
                _usersService.CreateOrUpdateUser(user);

                return Ok();
            }
            catch (Exception e)
            {
                var error = new ErrorDetail()
                {
                    ErrorMessage = e.Message
                };

                return StatusCode(500, error);
            }
        }

		// Get api/users
		// [AuthoriseWithApiKey]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpGet]
	    [Produces("application/json")]
		public IActionResult Get()
	    {
		    try
		    {
			    return Ok(_usersService.GetUsers());
		    }
		    catch (Exception e)
		    {
			    var error = new ErrorDetail()
			    {
				    ErrorMessage = e.Message
			    };

			    return StatusCode(500, error);
		    }
	    }

		// Delete api/users
		//[AuthoriseWithApiKey]
	    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "administration")]
		[HttpDelete]
	    [Produces("application/json")]
	    public IActionResult Delete(int userId)
	    {
		    try
		    {
			    _usersService.DeleteUser(userId);
				return Ok();
		    }
		    catch (Exception e)
		    {
			    var error = new ErrorDetail()
			    {
				    ErrorMessage = e.Message
			    };

			    return StatusCode(500, error);
		    }
	    }
	}
}