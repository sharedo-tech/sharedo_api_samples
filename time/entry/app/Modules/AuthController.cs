using App.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace App.Modules
{
    [ApiController]
    [Route("/api")]
    public class LogoutController : ControllerBase
    {
        private readonly ITokenManager _tokenManager;

        public LogoutController(ITokenManager tokenManager)
        {
            _tokenManager = tokenManager;
        }

        [HttpGet("login")]
        public IActionResult Login(string returnUrl="/")
        {
            return new ChallengeResult(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = returnUrl
            });
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            if( User.Identity?.IsAuthenticated ?? false )
            {
                await _tokenManager.RevokeTokensAsync();
                await HttpContext.SignOutAsync();
            }

            return Redirect("/");
        }
    }
}