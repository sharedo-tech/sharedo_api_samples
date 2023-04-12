using Filing.App.Sharedo;
using Filing.App.Sharedo.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Filing.App.Pages
{
    public class WorkItemModel : PageModel
    {
        private readonly IWorkItemSearches _workItemSearches;

        private const string DocumentExpectationPendingPhase = "task-activity-document-expectation-expectation";

        public WorkItemModel(
            IWorkItemSearches workItemSearches)
        {
            _workItemSearches = workItemSearches;
        }

        public WorkItem? WorkItem { get; private set; } 

        public IList<WorkItem> PendingExpectations { get; private set; } = new List<WorkItem>(0);

        public async Task<IActionResult> OnGet(string id)
        {
            if (!Guid.TryParse(id, out var workItemId))
            {
                return Redirect("/");
            }

            WorkItem = (await _workItemSearches.FindByIds(new List<Guid> { workItemId }))
                .FirstOrDefault();

            var expectations = await _workItemSearches.FindDocumentExpectationForMatter(workItemId);

            PendingExpectations = expectations.Where(e => e.PhaseName == DocumentExpectationPendingPhase).ToList();

            return Page();
        }
    }
}
