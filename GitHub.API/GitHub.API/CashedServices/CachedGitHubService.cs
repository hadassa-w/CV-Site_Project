using GitHub.Service;
using Microsoft.Extensions.Caching.Memory;
using Octokit;

namespace GitHub.API.CashedServices
{
    public class CachedGitHubService : IGitHubService
    {
        private readonly IGitHubService _gitHubService;
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        private const string UserPortfolioKey = "UserPortfolioKey";

        public CachedGitHubService(IGitHubService gitHubService, IMemoryCache memoryCache)
        {
            _gitHubService = gitHubService;
            _memoryCache = memoryCache;
        }

        public Task<IEnumerable<Repository>> GetUserRepositories(string username)
        {
            return _gitHubService.GetUserRepositories(username);
        }

        public Task<IEnumerable<object>> SearchRepositories(string repositoryName = null, string language = null, string username = null)
        {
            return _gitHubService.SearchRepositories(repositoryName, language, username);
        }

        public async Task<IEnumerable<object>> GetPortfolio()
        {

            if (_memoryCache.TryGetValue(UserPortfolioKey, out IEnumerable<object> cachedPortfolio))
            {
                return cachedPortfolio!;
            }

            var portfolio = await _gitHubService.GetPortfolio();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
            .SetSlidingExpiration(TimeSpan.FromMinutes(10));

            portfolio = await _gitHubService.GetPortfolio();
            _memoryCache.Set(UserPortfolioKey, portfolio, cacheEntryOptions);

            return portfolio;
        }
    }
}
