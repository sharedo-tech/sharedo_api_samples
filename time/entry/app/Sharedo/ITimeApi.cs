using App.Sharedo.Models;

namespace App.Sharedo
{
    public interface ITimeApi
    {
        Task<IEnumerable<Category>> GetCategories();
        Task<CaptureDefinition> GetCapture(string category, Guid workItemId);
        Task SendTime(TimeEntryDto dto);
    }
}

