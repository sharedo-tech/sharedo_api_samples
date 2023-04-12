using Microsoft.AspNetCore.Authentication;

namespace Filing.App.Auth
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthHeaderHandler(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                throw new UnauthorizedAccessException();
            }

            var accessToken = await context.GetTokenAsync("access_token");

            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
