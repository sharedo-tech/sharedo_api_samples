using Filing.App.Sharedo.Contracts;

namespace Filing.App.Sharedo
{
    public interface IDocumentManagement
    {
        /// <summary>
        /// Get a collection of folder for the path.
        /// </summary>
        /// <param name="workItemId">The work item id.</param>
        /// <param name="path">The path to get folders for.</param>
        /// <returns>A collection of folders.</returns>
        Task<IReadOnlyList<Folder>> GetFolders(Guid workItemId, string path);

        /// <summary>
        /// Upload a document against a document expectation.
        /// </summary>
        /// <param name="documentExpectationWorkItemId">The document expection work item id.</param>
        /// <param name="file">The file to upload.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="folder">The folder path to upload to.</param>
        /// <returns>The related document id.</returns>
        Task<Guid> UploadDocument(Guid documentExpectationWorkItemId, MemoryStream file, string filename, string folder);

        /// <summary>
        /// Set the phase for receiving document on document expectation work item.
        /// </summary>
        /// <param name="documentExpectationWorkItemId">The document expection work item id.</param>
        /// <returns>The new phase.</returns>
        Task<Phase> ReceiveDocument(Guid documentExpectationWorkItemId);
    }
}
