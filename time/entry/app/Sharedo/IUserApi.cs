namespace App.Sharedo
{
    public interface IUserApi
    {
        Task<Guid> GetCurrentUserId();
    }
}