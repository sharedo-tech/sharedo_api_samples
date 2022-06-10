using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Modules
{
    [ApiController]
    [Route("/api/profile")]
    public class ProfileController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public IActionResult GetProfile()
        {
            return new JsonResult(new
            {
                User = User.Identity?.Name
            });
        }
    }
}