using System.Text.Json.Nodes;
using Filing.App.Sharedo.Contracts;

namespace Filing.App.Sharedo
{
    public class WorkItemSearches : IWorkItemSearches
    {
        private readonly HttpClient _httpClient;
        private readonly object[] EnrichmentFields = new[]
                {
                    new { Path = "id" },
                    new { Path = "title" },
                    new { Path = "reference" },
                    new { Path = "phase.systemName" },
                    new { Path = "type.systemName" },
                };

        public WorkItemSearches(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(Clients.SharedoApi);
        }

        /// <inheritdoc />
        public async Task<IList<WorkItem>> FindByIds(IList<Guid> workItemIds)
        {
            var searchRequest = new
            {
                Search = new
                {
                    WorkItemIds = workItemIds,
                },
                Enrich = EnrichmentFields,
            };

            return await Search(searchRequest);
        }

        /// <inheritdoc />
        public async Task<IList<WorkItem>> FindMattersByTitle(string title)
        {
            var searchRequest = new
            {
                Search = new
                {
                    Title = title,
                    Types = new
                    {
                        IncludeTypesDerivedFrom = new[] { "matter" }
                    },
                },
                Enrich = EnrichmentFields,
            };

            return await Search(searchRequest);
        }

        public async Task<IList<WorkItem>> FindMattersByReference(string reference)
        {
            var searchRequest = new
            {
                Search = new
                {
                    Reference = reference,
                    Types = new
                    {
                        IncludeTypesDerivedFrom = new[] { "matter" }
                    },
                },
                Enrich = EnrichmentFields,
            };

            return await Search(searchRequest);
        }

        /// <inheritdoc />
        public async Task<IList<WorkItem>> FindMattersByClientId(Guid clientId)
        {
            var searchRequest = new
            {
                Search = new
                {
                    Roles = new[]
                    {
                        new
                        {
                            Role = "client",
                            SubjectIdsHoldingRole = new[]
                            {
                                clientId,
                            },
                        },
                    },
                    Types = new
                    {
                        IncludeTypesDerivedFrom = new[] { "matter" }
                    },
                },
                Enrich = EnrichmentFields,
            };

            return await Search(searchRequest);
        }

        /// <inheritdoc />
        public async Task<IList<WorkItem>> FindMattersByClientName(string clientName)
        {
            var searchRequest = new
            {
                Search = new
                {
                    Roles = new[]
                    {
                        new
                        {
                            Role = "client",
                            Search = clientName,
                        },
                    },
                    Types = new
                    {
                        IncludeTypesDerivedFrom = new[] { "matter" }
                    },
                },
                Enrich = EnrichmentFields,
            };

            return await Search(searchRequest);
        }

        /// <inheritdoc />
        public async Task<IList<WorkItem>> FindMattersByRolesWithPostCode(string[] roles, string postCode)
        {
            var searchRequest = new
            {
                Search = new
                {
                    AdvancedRoles = new[]
                    {
                        new
                        {
                            Roles = roles,
                            PostCode = postCode,
                        },
                    },
                    Types = new
                    {
                        IncludeTypesDerivedFrom = new[] { "matter" }
                    },
                },
                Enrich = EnrichmentFields,
            };

            return await Search(searchRequest);
        }

        public async Task<IList<WorkItem>> FindMattersByEmail(string email)
        {
            var searchRequest = new
            {
                Search = new
                {
                    AdvancedRoles = new[]
                    {
                        new
                        {
                            ContactDetails = new[]
                            {
                                new
                                {
                                    ContactType = "email",
                                    Value = email,
                                },
                            },
                        },
                    },
                    Types = new
                    {
                        IncludeTypesDerivedFrom = new[] { "matter" }
                    },
                },
                Enrich = EnrichmentFields,
            };

            return await Search(searchRequest);
        }

        /// <inheritdoc />
        public async Task<IList<WorkItem>> FindMattersByAttribute(string key, string value)
        {
            var searchRequest = new
            {
                Search = new
                {
                    Attributes = new[]
                    {
                        new {
                            Key = key,
                            Value = value,
                        },
                    },
                    Types = new
                    {
                        IncludeTypesDerivedFrom = new[] { "matter" }
                    },
                },
                Enrich = EnrichmentFields,
            };

            return await Search(searchRequest);
        }

        public async Task<IList<WorkItem>> FindDocumentExpectationForMatter(Guid workItemId)
        {
            var searchRequest = new
            {
                Search = new
                {
                    Types = new
                    {
                        IncludeTypes = new[]
                        {
                            "task-activity-document-expectation"
                        },
                    },
                    Graph = new
                    {
                        AncestorIds = new[]
                        {
                            workItemId
                        },
                        MaxAncestorDistance = 1,
                    },
                },
                Enrich = EnrichmentFields,
            };

            return await Search(searchRequest);
        }

        private async Task<IList<WorkItem>> Search(object searchRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/v1/public/workitem/findByQuery", searchRequest);

            await response.EnsureSuccess();

            var jObject = JsonNode.Parse(await response.Content.ReadAsStringAsync());

            if (jObject == null)
            {
                throw new InvalidOperationException("Invalid Json when parsing search response");
            }

            var workItems = new List<WorkItem>();

            foreach (var result in jObject["results"]?.AsArray() ?? new JsonArray())
            {
                var data = result?["data"];

                if (data == null)
                {
                    continue;
                }

                if (!Guid.TryParse(data["id"]?.ToString(), out var id))
                {
                    continue;
                }

                workItems.Add(new WorkItem(
                    id, 
                    data["title"]?.ToString() ?? string.Empty,
                    data["reference"]?.ToString() ?? string.Empty,
                    data["phase.systemName"]?.ToString() ?? string.Empty,
                    data["type.systemName"]?.ToString() ?? string.Empty));
            }

            return workItems;
        }
    }
}
