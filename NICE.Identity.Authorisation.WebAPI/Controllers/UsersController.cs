using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NICE.Identity.Authorisation.WebAPI.ApiModels.Responses;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

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
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("/api/Users/{userId}")]
        [Produces("application/json")]
        public IActionResult Get(int userId)
        {
            try
            {
                return Ok(_usersService.GetUser(userId));
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

        // POST api/users
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] User user)
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
                _usersService.CreateUser(user);

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

		// Delete api/users
		//[AuthoriseWithApiKey]
	    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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