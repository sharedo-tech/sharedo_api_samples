namespace Filing.App.Auth
{
    public interface ITokenRefesher
    {
        Task<Token> Refresh(string refreshToken);
    }
}
