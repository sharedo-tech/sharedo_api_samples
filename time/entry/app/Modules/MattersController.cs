using App.Sharedo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Modules
{
    [ApiController]
    [Route("/api/matters")]
    public class MattersController : ControllerBase
    {
        private readonly IUserApi _userInfo;
        private readonly IMyMattersApi _matters;

        public MattersController(IUserApi userInfo, IMyMattersApi matters)
        {
            _userInfo = userInfo;
            _matters = matters;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMatterList()
        {
            var userId = await _userInfo.GetCurrentUserId();
            var matters = await _matters.GetUserMatters(userId);

            return new JsonResult(matters);
        }
    }
}