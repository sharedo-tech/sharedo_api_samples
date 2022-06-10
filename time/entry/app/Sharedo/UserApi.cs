using App.Infrastructure;
using App.Sharedo.Models;

namespace App.Sharedo
{
    public class UserApi : IUserApi
    {
        private readonly IHttpTokenClient _tokenClient;

        public UserApi(IHttpTokenClient tokenClient)
        {
            _tokenClient = tokenClient;
        }
        
        public async Task<Guid> GetCurrentUserId()
        {
            var request = new HttpRequestMessage
            (
                HttpMethod.Get, 
                $"{Program.SharedoUrl}/api/security/userInfo"
            );
            request.Headers.Add("accept", "application/json");

            var response = await _tokenClient.SendWithTokensAsync(request);
            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadFromJsonAsync<UserInfoResponse>();
            if (user == null) throw new InvalidOperationException();

            return user.UserId ?? Guid.Empty;
        }
    }
}