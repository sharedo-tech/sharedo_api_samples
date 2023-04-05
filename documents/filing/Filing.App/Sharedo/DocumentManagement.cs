using System.Net.Http.Json;
using Filing.App.Sharedo.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace Filing.App.Sharedo
{
    public class DocumentManagement : IDocumentManagement
    {
        private readonly HttpClient _httpClient;

        public DocumentManagement(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(Clients.SharedoApi);
        }

        public async Task<IReadOnlyList<Folder>> GetFolders(Guid workItemId, string path)
        {
            var response = await _httpClient.GetAsync($"/api/v1/public/workItem/{workItemId}/dms/{path}");

            await response.EnsureSuccess();

            var folderContents = await response.Content.ReadFromJsonAsync<FolderContents>() ??
                new FolderContents(new List<Folder>(0));

            return folderContents.Folders;
        }

        public async Task<Guid> UploadDocument(Guid documentExpectationWorkItemId, MemoryStream file, string filename, string folder)
        {
            using var content = new MultipartFormDataContent();

            var fileContent = new StreamContent(file);

            content.Add(new StreamContent(file), "file", filename);

            content.Add(new StringContent(folder), "folder");

            var response = await _httpClient
                .PostAsync($"/api/v1/public/workitem/{documentExpectationWorkItemId}/relatedDocuments", content);

            var res = await response.Content.ReadFromJsonAsync<UploadDocumentResult>() ??
                throw new InvalidOperationException("Unable to read upload result from response"); ;

            await response.EnsureSuccess();

            return res.RelatedDocumentIds.First();
        }

        public async Task<Phase> ReceiveDocument(Guid documentExpectationWorkItemId)
        {
            var request = new
            {
                // The name of phase to transition to once related document has been uploaded
                ToPhaseSystemName = "task-activity-document-expectation-reviewing",
                Description = "Automated by Filing App",
            };

            var response = await _httpClient.PutAsJsonAsync($"/api/v1/public/workItem/{documentExpectationWorkItemId}/phase", request);

            await response.EnsureSuccess();

            var phase = await response.Content.ReadFromJsonAsync<Phase>() ??
                throw new InvalidOperationException("Unable to read phase from response");

            return phase;
        }
    }
}
