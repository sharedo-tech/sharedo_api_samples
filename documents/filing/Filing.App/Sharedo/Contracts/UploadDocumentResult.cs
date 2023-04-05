namespace Filing.App.Sharedo.Contracts
{
    public record UploadDocumentResult(IReadOnlyList<Guid> RelatedDocumentIds)
    {
    }
}
