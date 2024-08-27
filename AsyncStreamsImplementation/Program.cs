using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octokit;

namespace AsyncStreamsImplementation
{
    class ProgressStatus : IProgress<int>
    {
        Action<int> action;

        public ProgressStatus(Action<int> progressAction) =>
            action = progressAction;

        public void Report(int value) => action(value);
    }

    public class GraphQLRequest
    {
        [JsonProperty("query")] public string? Query { get; set; }

        [JsonProperty("variables")]
        public IDictionary<string, object> Variables { get; } = new Dictionary<string, object>();

        public string ToJsonText() =>
            JsonConvert.SerializeObject(this);
    }

    public static class Program
    {
        static async Task Main(string[] args)
        {
            var key = GetEnvVariable("GitHubKey", "You must use Environment Variables to get the GitHub API key", "");

            var client = new Octokit.GitHubClient(new ProductHeaderValue("AsyncStreamsImplementation"))
            {
                Credentials = new Credentials(key)
            };

            var progressReported =
                new ProgressStatus((num) => { Console.WriteLine($"Received {num} issues in total"); });

            CancellationTokenSource cancellationTokenSource = new();

            try
            {
                var results = await RunPagedQueryAsync(client, PagedIssueQuery, "docs",
                    cancellationTokenSource.Token, progressReported);
                foreach(var issue in results)
                    Console.WriteLine(issue);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Work has been cancelled!");
            }
        }

        private static string GetEnvVariable(string item, string error, string defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(item);
            if (string.IsNullOrWhiteSpace(value))
            {
                if (!string.IsNullOrWhiteSpace(defaultValue))
                {
                    return defaultValue;
                }

                if (!string.IsNullOrWhiteSpace(error))
                {
                    Console.WriteLine(error);
                    Environment.Exit(0);
                }
            }

            return value ?? string.Empty;
        }

        private static async Task<JArray> RunPagedQueryAsync(GitHubClient client, string queryText, string repoName,
            CancellationToken cancel, IProgress<int> progress)
        {
            var issueAndPRQuery = new GraphQLRequest
            {
                Query = queryText
            };
            issueAndPRQuery.Variables["repo_name"] = repoName;

            JArray finalResults = new JArray();
            bool hasMorePages = true;
            int pagesReturned = 0;
            int issuesReturned = 0;

            // Stop with 10 pages, because these are large repos:
            while (hasMorePages && (pagesReturned++ < 10))
            {
                var postBody = issueAndPRQuery.ToJsonText();
                var response = await client.Connection.Post<string>(new Uri("https://api.github.com/graphql"),
                    postBody, "application/json", "application/json");

                JObject results = JObject.Parse(response.HttpResponse.Body.ToString()!);

                int totalCount = (int)issues(results)["totalCount"]!;
                hasMorePages = (bool)pageInfo(results)["hasPreviousPage"]!;
                issueAndPRQuery.Variables["start_cursor"] = pageInfo(results)["startCursor"]!.ToString();
                issuesReturned += issues(results)["nodes"]!.Count();
                // <SnippetProcessPage>
                finalResults.Merge(issues(results)["nodes"]!);
                progress?.Report(issuesReturned);
                cancel.ThrowIfCancellationRequested();
                // </SnippetProcessPage>
            }

            return finalResults;

            JObject issues(JObject result) => (JObject)result["data"]!["repository"]!["issues"]!;
            JObject pageInfo(JObject result) => (JObject)issues(result)["pageInfo"]!;
        }

        private const string PagedIssueQuery =
            @"query ($repo_name: String!,  $start_cursor:String) {
              repository(owner: ""dotnet"", name: $repo_name) {
                issues(last: 25, before: $start_cursor)
                 {
                    totalCount
                    pageInfo {
                      hasPreviousPage
                      startCursor
                    }
                    nodes {
                      title
                      number
                      createdAt
                    }
                  }
                }
              }
            ";
    }
}