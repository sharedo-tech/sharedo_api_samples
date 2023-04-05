using Filing.App.Sharedo;
using Filing.App.Sharedo.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Filing.App.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public const string ViewMostRecent = "most-recent";
        public const string ViewMostViewed = "most-viewed";
        public const string ViewBookmarked = "bookmarked";

        private readonly IMyWorkItems _myWorkItems;
        private readonly IWorkItemSearches _workItemSearches;

        public IndexModel(
            IMyWorkItems myWorkItems,
            IWorkItemSearches workItemSearches)
        {
            _myWorkItems = myWorkItems;
            _workItemSearches = workItemSearches;
        }

        public IList<WorkItem> WorkItems { get; private set; } = new List<WorkItem>(0);

        public string Description { get; private set; } = string.Empty;

        public async Task<IActionResult> OnGet(string viewType)
        {
            var ids = viewType switch
            {
                ViewMostViewed => await _myWorkItems.GetMostViewed(10),
                ViewBookmarked => await _myWorkItems.GetBookmarked(10),
                _ => await _myWorkItems.GetMostRecent(10),
            };

            WorkItems = await _workItemSearches.FindByIds(ids);
            Description = GetDescription(viewType, WorkItems.Count);

            ViewData["ViewType"] = viewType;
            return Page();
        }

        private static string GetDescription(string viewType, int count)
        {
            return viewType switch
            {
                ViewMostRecent => $"{count} most recent work items",
                ViewMostViewed => $"{count} most viewed work items",
                ViewBookmarked => $"{count} bookmarked work items",
                _ => string.Empty,
            };
        }
    }
}
