using App.Infrastructure;
using App.Sharedo.Models;

namespace App.Sharedo
{
    public class TimeApi : ITimeApi
    {
        private readonly IHttpTokenClient _tokenClient;

        public TimeApi(IHttpTokenClient tokenClient)
        {
            _tokenClient = tokenClient;
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            var request = new HttpRequestMessage
            (
                HttpMethod.Get,
                $"{Program.SharedoUrl}/api/v2/public/time/categories"
            );
            request.Headers.Add("accept", "application/json");

            var response = await _tokenClient.SendWithTokensAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Category>>();
        }

        public async Task<CaptureDefinition> GetCapture(string category, Guid workItemId)
        {
            var request = new HttpRequestMessage
            (
                HttpMethod.Get,
                $"{Program.SharedoUrl}/api/v2/public/time/capture/{category}/{workItemId}"
            );
            request.Headers.Add("accept", "application/json");

            var response = await _tokenClient.SendWithTokensAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CaptureDefinition>();
        }

        public async Task SendTime(TimeEntryDto dto)
        {
            var request = new HttpRequestMessage
            (
                HttpMethod.Post,
                $"{Program.SharedoUrl}/api/v2/public/time/entry"
            );
            request.Headers.Add("accept", "application/json");
            
            request.Content = JsonContent.Create(dto);

            var response = await _tokenClient.SendWithTokensAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}