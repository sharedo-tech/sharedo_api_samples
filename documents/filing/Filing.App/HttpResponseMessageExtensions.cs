public static class HttpResponseMessageExtensions
{
    public static async Task EnsureSuccess(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = await response.Content.ReadAsStringAsync();

        throw new HttpRequestException($"Error sending request to url:{response.RequestMessage?.RequestUri}, Status:{response.StatusCode}\n{content}", null, response.StatusCode);
    }
}
