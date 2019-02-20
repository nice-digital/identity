using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NICE.Identity.Authorisation.WebAPI.ApiModels.Responses;
using NICE.Identity.Authorisation.WebAPI.Services;
using User = NICE.Identity.Authorisation.WebAPI.ApiModels.Requests.User;

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
	        _logger = logger;
	        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }

        // PUT api/users
        [HttpPut]
        public async Task<ActionResult> Put([FromBody] User user)
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
    }
}