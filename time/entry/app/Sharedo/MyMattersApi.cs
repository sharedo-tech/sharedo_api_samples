using App.Infrastructure;
using App.Sharedo.Models;

namespace App.Sharedo
{
    public class MyMattersApi : IMyMattersApi
    {
        private readonly IHttpTokenClient _tokenClient;

        public MyMattersApi(IHttpTokenClient tokenClient)
        {
            _tokenClient = tokenClient;
        }

        public async Task<IEnumerable<MatterInfo>> GetUserMatters(Guid userId)
        {
            var request = new HttpRequestMessage
            (
                HttpMethod.Post, 
                $"{Program.SharedoUrl}/api/v1/public/workItem/findByQuery"
            );
            request.Headers.Add("accept", "application/json");

            request.Content = JsonContent.Create(new
            {
                Search = new
                {
                    Page = new
                    {
                        Page = 1,
                        RowsPerPage = 5
                    },
                    Sort = new
                    {
                        OrderBy = "updatedDate",
                        Direction = "descending"
                    },
                    Phase = new
                    {
                        IncludeOpen = true,
                        IncludeClosed = false,
                        IncludeRemoved = false
                    },
                    Types = new
                    {
                        IncludeTypesDerivedFrom = new[] { "matter" }
                    },
                    Ownership = new
                    {
                        MyScope = new
                        {
                            OwnerIds = new[] { userId },
                            Primary = true,
                            Secondary = false
                        }
                    }
                },
                Enrich = new []
                {
                    new { Path = "reference" },
                    new { Path = "title" }
                }
            });

            var response = await _tokenClient.SendWithTokensAsync(request);
            response.EnsureSuccessStatusCode();

            var queryResponse = await response.Content.ReadFromJsonAsync<FindByQueryResponse>();
            if (queryResponse == null) throw new InvalidOperationException();

            return queryResponse.Results.Select(item => new MatterInfo
            {
                Id = item.Id,
                Reference = item.Data.Reference,
                Title = item.Data.Title,
                Url = $"{Program.SharedoUrl}/sharedo/{item.Id}"
            });
        }
    }
}