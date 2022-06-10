namespace App.Sharedo.Models
{
    public class FindByQueryResponse
    {
        public int TotalCount { get; set; }
        public List<FindByQueryItem> Results{ get; set; }

        public FindByQueryResponse()
        {
            Results = new List<FindByQueryItem>();
        }
    }
}