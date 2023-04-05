using Filing.App.Sharedo;
using Filing.App.Sharedo.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Filing.App.Pages
{
    public class SearchModel : PageModel
    {
        private readonly IWorkItemSearches _workItemSearches;

        public SearchModel(
            IWorkItemSearches workItemSearches)
        {
            _workItemSearches = workItemSearches;
        }

        [BindProperty]
        public SearchType SearchType { get; set; } = SearchType.Title;

        [BindProperty]
        public string SearchTerm { get; set; } = string.Empty;

        public IEnumerable<SearchType> SearchTypes { get; } = Enum.GetValues<SearchType>();

        public IList<WorkItem> WorkItems { get; set; } = new List<WorkItem>(0);

        public void OnGet()
        {
        }

        public async Task OnPost()
        {
            WorkItems = await GetWorkItems(SearchType, SearchTerm);

            SearchTerm = string.Empty;
        }

        private async Task<IList<WorkItem>> GetWorkItems(SearchType searchType, string searchTerm)
        {
            searchTerm = searchTerm.Trim();

            switch (searchType)
            {
                case SearchType.Title:
                    return await _workItemSearches.FindMattersByTitle(searchTerm);
                case SearchType.Reference:
                    return await _workItemSearches.FindMattersByReference(searchTerm);
                case SearchType.ClientId:
                    if(!Guid.TryParse(searchTerm, out var clientId))
                    {
                        return new List<WorkItem>(0);
                    }
                    return await _workItemSearches.FindMattersByClientId(clientId);
                case SearchType.ClientName:
                    return await _workItemSearches.FindMattersByClientName(searchTerm);
                case SearchType.PostCode:
                    return await _workItemSearches.FindMattersByPostCode(searchTerm);
                case SearchType.Email:
                    return await _workItemSearches.FindMattersByEmail(searchTerm);
                case SearchType.Attribute:
                    var parts = searchTerm.Split("=");
                    if(parts.Length < 2)
                    {
                        return new List<WorkItem>(0);
                    }
                    return await _workItemSearches.FindMattersByAttribute(parts[0], parts[1]);
                default:
                    return new List<WorkItem>(0);
            }
        }
    }
    public enum SearchType
    {
        Title,
        Reference,
        ClientId,
        ClientName,
        PostCode,
        Email,
        Attribute,
    }
}
