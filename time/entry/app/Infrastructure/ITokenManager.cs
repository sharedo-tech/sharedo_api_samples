namespace App.Infrastructure
{
    public interface ITokenManager
    {
        /// <summary>
        /// Get the current access token
        /// </summary>
        Task<string> GetAccessTokenFromContextAsync();

        /// <summary>
        /// Get the current refresh token
        /// </summary>
        Task<string> GetRefreshTokenFromContextAsync();

        /// <summary>
        /// Use the refresh token to get a new access token
        /// </summary>
        Task<string> GetNewAccessTokenAsync();

        /// <summary>
        /// Attempt to revoke the current access token (if it's a reference token)
        /// and refresh token. These will expire in short order anyway when not
        /// being used, but it's good practice to kill them...
        /// </summary>
        Task RevokeTokensAsync();
    }
}