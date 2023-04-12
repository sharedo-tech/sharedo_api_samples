namespace Filing.App.Sharedo.Contracts
{
    public record FolderContents(IReadOnlyList<Folder> Folders)
    {
    }

    public record Folder(string Name, string PathId)
    {
    }
}
