using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace golbym.Api.Controllers
{
	[Authorize]
	[Route("api/test")]
	[ApiController]
	public class TestController : ControllerBase
	{
		[Route("hello")]
		[HttpGet]
		public ActionResult<string> Hello()
		{
			return Ok("Hello");
		}
	}
}
