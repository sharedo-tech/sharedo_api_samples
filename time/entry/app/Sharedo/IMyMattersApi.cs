using App.Sharedo.Models;

namespace App.Sharedo
{
    public interface IMyMattersApi
    {
        Task<IEnumerable<MatterInfo>> GetUserMatters(Guid userId);
    }
}