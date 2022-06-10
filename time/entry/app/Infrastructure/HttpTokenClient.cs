using System.Net;

namespace App.Infrastructure
{
    public class HttpTokenClient : IHttpTokenClient
    {
        private readonly ITokenManager _tokens;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<HttpTokenClient> _log;

        public HttpTokenClient(ITokenManager tokens, IHttpClientFactory clientFactory, ILogger<HttpTokenClient> log)
        {
            _tokens = tokens;
            _clientFactory = clientFactory;
            _log = log;
        }

        public async Task<HttpResponseMessage> SendWithTokensAsync(HttpRequestMessage request)
        {
            // Grab the access token from the current authentication context
            var accessToken = await _tokens.GetAccessTokenFromContextAsync();
            var hasAccessToken = !string.IsNullOrWhiteSpace(accessToken);
            if (!hasAccessToken)
            {
                _log.LogDebug("Call to {url} - we do not have an access token - trying call anyway", request.RequestUri);
            }

            // Make the API call attempt
            _log.LogDebug("Calling {url} with access token - attempt 1", request.RequestUri);
            var response = await Send(request, accessToken);

            // If we didn't have an access token, or response was anything other than a 401 = we're done
            if (!hasAccessToken || response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response;
            }

            // We got a 401 - AT might be expired or revoked
            _log.LogDebug("Call to {url} with current access token was unauthorised - starting refresh", request.RequestUri);

            // Request new tokens
            accessToken = await _tokens.GetNewAccessTokenAsync();
            hasAccessToken = !string.IsNullOrWhiteSpace(accessToken);
            if (!hasAccessToken)
            {
                // Couldn't renew - just return original response
                _log.LogDebug("Call to {url} couldn't get a new access token", request.RequestUri);
                return response;
            }

            // Try another attempt now we have a new access token and just return 
            // it's response, come what may....
            _log.LogDebug("Got new access token - retrying {url}", request.RequestUri);
            return await Send(CloneRequest(request), accessToken);
        }

        private Task<HttpResponseMessage> Send(HttpRequestMessage request, string accessToken)
        {
            request.Headers.Remove("Authorization");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            return _clientFactory.CreateClient().SendAsync(request);
        }

        private HttpRequestMessage CloneRequest(HttpRequestMessage original)
        {
            // Wish the API could just do this for us!
            var request = new HttpRequestMessage(original.Method, original.RequestUri);
            request.Content = request.Content;
            request.Headers.Clear();
            foreach (var h in request.Headers)
            {
                request.Headers.Add(h.Key, h.Value);
            }

            return request;
        }
    }
}