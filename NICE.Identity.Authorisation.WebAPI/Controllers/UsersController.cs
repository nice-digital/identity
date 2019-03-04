using System;
using System.Threading.Tasks;
using System.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NICE.Identity.Authorisation.WebAPI.ApiModels.Responses;
using NICE.Identity.Authorisation.WebAPI.Common;
using NICE.Identity.Authorisation.WebAPI.Services;
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
		[AuthoriseWithApiKey]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateUser user)
        {
            if (!ModelState.IsValid)
            {
	            var serializableModelState = new SerializableError(ModelState);
	            var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
	            _logger.LogError($"Invalid Model State at UsersController.Put for user id: {user.UserId}", modelStateJson);

				return BadRequest("Request failed validation");
            }

            try
            {
                await _usersService.CreateUser(user);

                return Ok();
            }
            catch (Exception e)
            {
                var error = new ErrorDetail()
                {
                    ErrorMessage = e.Message
                };

                return StatusCode(503, error);
            }
        }

	    // Get api/users
	    [AuthoriseWithApiKey]
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

			    return StatusCode(503, error);
		    }
	    }
	}
}