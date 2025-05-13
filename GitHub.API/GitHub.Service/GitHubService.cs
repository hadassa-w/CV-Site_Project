using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using Microsoft.Extensions.Options;

namespace GitHub.Service
{
    public class GitHubService : IGitHubService
    {
        private readonly GitHubClient _client;
        private readonly string _userName;

        public GitHubService(IOptions<GitHubIntegrationOptions> options)
        {
            var settings = options.Value;
            _userName = settings.UserName;

            var credentials = new Credentials(settings.Token);
            _client = new GitHubClient(new Octokit.ProductHeaderValue("MyGitHubApp"))
            {
                Credentials = credentials
            };
        }

        public async Task<IEnumerable<Repository>> GetUserRepositories(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Username is required");

            return await _client.Repository.GetAllForUser(username);
        }

        public async Task<IEnumerable<object>> GetPortfolio()
        {
            var repositories = await _client.Repository.GetAllForUser(_userName);

            var result = new List<object>();

            foreach (var repo in repositories)
            {
                var languages = await _client.Repository.GetAllLanguages(repo.Owner.Login, repo.Name);
                var commits = await _client.Repository.Commit.GetAll(repo.Owner.Login, repo.Name);
                var pulls = await _client.PullRequest.GetAllForRepository(repo.Owner.Login, repo.Name);

                result.Add(new
                {
                    repo.Name,
                    repo.Description,
                    Languages = languages.Select(l => l.Name),
                    LastCommitDate = commits.FirstOrDefault()?.Commit.Committer.Date.UtcDateTime,
                    repo.StargazersCount,
                    PullRequestCount = pulls.Count,
                    repo.HtmlUrl
                });
            }

            return result;
        }

        public async Task<IEnumerable<object>> SearchRepositories(string repositoryName = null, string language = null, string username = null)
        {
            var searchRequest = new SearchRepositoriesRequest(repositoryName)
            {
                Language = language != null ? Enum.TryParse<Language>(language, out var lang) ? lang : null : null,
                User = username
            };

            var searchResult = await _client.Search.SearchRepo(searchRequest);

            var result = searchResult.Items.Select(repo => new
            {
                repo.Name,
                repo.Description,
                repo.Language,
                repo.StargazersCount,
                repo.HtmlUrl
            });

            return result;
        }
    }
}

