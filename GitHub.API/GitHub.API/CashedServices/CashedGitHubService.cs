using GitHub.Service;
using Microsoft.Extensions.Caching.Memory;
using Octokit;

namespace GitHub.API.CashedServices
{
    public class CashedGitHubService : IGitHubService
    {
        private readonly IGitHubService _gitHubService;
        private readonly IMemoryCache _memoryCache;

        public CashedGitHubService(IGitHubService gitHubService,IMemoryCache memoryCache)
        {
            _gitHubService = gitHubService;
            _memoryCache = memoryCache;
        }

        public Task<IEnumerable<object>> GetPortfolio()
        {
            return _gitHubService.GetPortfolio();
        }

        public Task<IEnumerable<Repository>> GetUserRepositories(string username)
        {
            return _gitHubService.GetUserRepositories(username);
        }

        public Task<IEnumerable<object>> SearchRepositories(string repositoryName = null, string language = null, string username = null)
        {
            return _gitHubService.SearchRepositories(repositoryName, language, username);
        }
    }
}
