using Microsoft.AspNetCore.Mvc;

namespace NICE.Identity.Areas.Admin.Controllers
{

	[Area("Admin")]
	public abstract class AdministrationControllerBase : Controller
	{
	}

	[Area("Admin")]
	public abstract class AdministrationAPIControllerBase : ControllerBase
	{
	}
}
