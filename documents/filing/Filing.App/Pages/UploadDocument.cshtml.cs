using System.ComponentModel.DataAnnotations;
using Filing.App.Sharedo;
using Filing.App.Sharedo.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Filing.App.Pages
{
    public class UploadDocumentModel : PageModel
    {
        private readonly IDocumentManagement _documentManagement;
        private readonly IWorkItemSearches _workItemsSearches;

        public UploadDocumentModel(
            IDocumentManagement documentManagement,
            IWorkItemSearches workItemSearches)
        {
            _documentManagement = documentManagement;
            _workItemsSearches = workItemSearches;
        }

        public WorkItem? WorkItem { get; set; }

        [Required(ErrorMessage = "Please select a file to upload")]
        public IFormFile? FormFile { set; get; }

        [Required]
        [BindProperty]
        public string? Folder { get; set; }

        public IReadOnlyList<Folder> Folders { get; set; } = new List<Folder>(0);

        public async Task OnGet(Guid id, Guid workItemId)
        {
            ViewData["id"] = id;
            ViewData["workItemId"] = workItemId;

            WorkItem = (await _workItemsSearches.FindByIds(new List<Guid> { id }))
                .FirstOrDefault();

            Folders = await _documentManagement.GetFolders(id, "/");
        }

        public async Task<IActionResult> OnPost(Guid id, Guid workItemId)
        {
            if (FormFile == null || FormFile.Length == 0 || string.IsNullOrEmpty(Folder))
            {
                return Page();
            }

            // Don't trust the filename, requires validation.
            Guid relatedDocumentId;

            using (var ms = new MemoryStream())
            {
                await FormFile.CopyToAsync(ms);

                relatedDocumentId = await _documentManagement.UploadDocument(id, ms, FormFile.FileName, Folder!);

                await _documentManagement.ReceiveDocument(id);
            }

            return Redirect($"/WorkItem?id={workItemId}");
        }
    }
}
