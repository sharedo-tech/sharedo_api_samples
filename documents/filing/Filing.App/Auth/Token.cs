namespace Filing.App.Auth
{
    public class Token
    {
        public Token(string accessToken, string refreshToken, int expires_in)
        {
            this.AccessToken = accessToken;
            this.RefreshToken = refreshToken;

            this.ExpiresOn = DateTime.UtcNow.AddSeconds(expires_in - 30);

            this.ErrorMessage = string.Empty;
        }

        public Token(string errorMessage)
        {
            this.AccessToken = string.Empty;
            this.RefreshToken = string.Empty;
            this.IsError = !string.IsNullOrEmpty(errorMessage);
            this.ErrorMessage = errorMessage;
        }

        public string AccessToken { get; }

        public string RefreshToken { get; }

        public DateTime ExpiresOn { get; }

        public bool IsError { get; }

        public string ErrorMessage { get; }
    }
}
