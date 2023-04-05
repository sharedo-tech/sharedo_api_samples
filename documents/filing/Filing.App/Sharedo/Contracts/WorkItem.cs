namespace Filing.App.Sharedo.Contracts
{
    public record WorkItem(Guid Id, string Title, string Reference, string PhaseName, string TypeName)
    {
        public override string ToString()
        {
            return $"{nameof(WorkItem)} Id:{Id}, Title:{Title}, Reference:{Reference}";
        }
    }
}
