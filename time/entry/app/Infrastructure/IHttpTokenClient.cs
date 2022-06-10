namespace App.Infrastructure
{
    public interface IHttpTokenClient
    {
        /// <summary>
        /// Sends a request and automatically manages the access token
        /// renewal once it becomes expired or invalid
        /// </summary>
        Task<HttpResponseMessage> SendWithTokensAsync(HttpRequestMessage request);
    }
}