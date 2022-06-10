using Microsoft.AspNetCore.Authentication;

namespace App.Infrastructure
{
    public class TokenManager : ITokenManager
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<TokenManager> _log;

        public TokenManager(IHttpClientFactory clientFactory, IHttpContextAccessor contextAccessor, ILogger<TokenManager> log)
        {
            _clientFactory = clientFactory;
            _contextAccessor = contextAccessor;
            _log = log;
        }

        public Task<string> GetAccessTokenFromContextAsync()
        {
            return GetTokenFromContext("access_token");
        }

        public Task<string> GetRefreshTokenFromContextAsync()
        {
            return GetTokenFromContext("refresh_token");
        }

        private Task<string> GetTokenFromContext(string tokenType)
        {
            var context = _contextAccessor.HttpContext;
            if (context == null) return Task.FromResult<string>(null);

            return context.GetTokenAsync(tokenType);
        }

        public async Task<string> GetNewAccessTokenAsync()
        {
            // Don't bother trying if we don't have a refresh token to use
            var refreshToken = await GetRefreshTokenFromContextAsync();
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                // We don't have a refresh token for some reason
                _log.LogDebug("Token manager could not obtain a new access token as no refresh token was issued");
                return null;
            }

            // Construct the request to /connect/token endpoint
            var tokenRequest = new HttpRequestMessage
            (
                HttpMethod.Post,
                $"{Program.IdentityUrl}/connect/token"
            )
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", refreshToken },
                    { "client_id", Program.ClientId },
                    { "client_secret", Program.ClientSecret }
                })
            };

            // Make the request
            var tokenClient = _clientFactory.CreateClient();
            var tokenResponse = await tokenClient.SendAsync(tokenRequest);

            if (!tokenResponse.IsSuccessStatusCode)
            {
                _log.LogWarning("Failed to get a new access token using current refresh token. {statusCode} {body}",
                    tokenResponse.StatusCode,
                    await tokenResponse.Content.ReadAsStringAsync());

                return null;
            }

            // Read it back
            var result = await tokenResponse.Content.ReadFromJsonAsync<TokenResponse>();

            // Store the new tokens back into the auth context
            await StoreTokens(result);

            // Send back the new access token
            return result.AccessToken;
        }

        private async Task StoreTokens(TokenResponse tokens)
        {
            var auth = await _contextAccessor.HttpContext.AuthenticateAsync();
            auth.Properties.UpdateTokenValue("access_token", tokens.AccessToken);
            auth.Properties.UpdateTokenValue("refresh_token", tokens.RefreshToken);
            await _contextAccessor.HttpContext.SignInAsync(auth.Principal, auth.Properties);
        }

        public async Task RevokeTokensAsync()
        {
            _log.LogDebug("Revoking access token");
            await Revoke("access_token");

            _log.LogDebug("Revoking refresh token");
            await Revoke("refresh_token");
        }

        private async Task Revoke(string tokenType)
        {
            var token = await GetTokenFromContext(tokenType);

            var tokenRequest = new HttpRequestMessage
            (
                HttpMethod.Post,
                $"{Program.IdentityUrl}/connect/revocation"
            )
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"client_id", Program.ClientId },
                    {"client_secret", Program.ClientSecret },
                    {"token", token},
                    {"token_type_hint", tokenType}
                })
            };

            var tokenClient = _clientFactory.CreateClient();
            var tokenResponse = await tokenClient.SendAsync(tokenRequest);

            if (!tokenResponse.IsSuccessStatusCode)
            {
                _log.LogWarning("Failed to revoke the {tokenType}", tokenType);
            }
        }
    }
}