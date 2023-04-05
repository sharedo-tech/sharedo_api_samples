using System.Net.Http.Json;

namespace Filing.App.Sharedo
{
    public class MyWorkItems : IMyWorkItems
    {
        private readonly HttpClient _httpClient;

        public MyWorkItems(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(Clients.SharedoApi);
        }

        /// <inheritdoc />
        public async Task<IList<Guid>> GetMostRecent(int count)
        {
            var response = await _httpClient.GetAsync($"/api/v1/public/my/workitems/most-recent/{count}");

            await response.EnsureSuccess();

            return await response.Content.ReadFromJsonAsync<List<Guid>>() ??
                new List<Guid>(0);
        }

        /// <inheritdoc />
        public async Task<IList<Guid>> GetMostViewed(int count)
        {
            var response = await _httpClient.GetAsync($"/api/v1/public/my/workitems/most-viewed/{count}");

            await response.EnsureSuccess();

            return await response.Content.ReadFromJsonAsync<List<Guid>>() ??
                new List<Guid>(0);
        }

        /// <inheritdoc />
        public async Task<IList<Guid>> GetBookmarked(int count)
        {
            var response = await _httpClient.GetAsync($"/api/v1/public/my/workitems/bookmarked/{count}");

            await response.EnsureSuccess();

            return await response.Content.ReadFromJsonAsync<List<Guid>>() ??
                new List<Guid>(0);
        }
    }
}
