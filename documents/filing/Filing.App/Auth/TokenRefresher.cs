using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;

namespace Filing.App.Auth
{
    public class TokenRefresher : ITokenRefesher
    {
        private readonly HttpClient _httpClient;
        private readonly SharedoAuthSettings _sharedoAuthSettings;

        public TokenRefresher(
            IHttpClientFactory httpClientFactory,
            IOptions<SharedoAuthSettings> sharedoAuthSettings)
        {
            _httpClient = httpClientFactory.CreateClient(Clients.SharedoIdentity);
            _sharedoAuthSettings = sharedoAuthSettings.Value;
        }

        public async Task<Token> Refresh(string refreshToken)
        {
            var body = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
                { "client_id", _sharedoAuthSettings.ClientId },
                { "client_secret", _sharedoAuthSettings.ClientSecret },
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"/connect/token");
            request.Headers.Add("accept", "application/json");
            request.Content = new FormUrlEncodedContent(body);

            var tokenResponse = await _httpClient.SendAsync(request);

            var jObject = JsonNode.Parse(await tokenResponse.Content.ReadAsStringAsync());

            var access_token = jObject?["access_token"]?.ToString();
            if (string.IsNullOrEmpty(access_token))
            {
                return new Token("No access_token in token response");
            }

            var refresh_token = jObject?["refresh_token"]?.ToString();
            if (string.IsNullOrEmpty(refresh_token))
            {
                return new Token("No refresh_token in token response");
            }

            var expires_in = jObject?["expires_in"]?.ToString();
            if (string.IsNullOrEmpty(expires_in))
            {
                return new Token("No expires_in in token response");
            }

            return new Token(access_token, refresh_token, int.Parse(expires_in));
        }
    }
}
