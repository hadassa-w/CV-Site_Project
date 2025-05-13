using Octokit;

namespace GitHub.Service
{
    public interface IGitHubService
    {
        Task<IEnumerable<Repository>> GetUserRepositories(string username);
        Task<IEnumerable<object>> GetPortfolio();
        Task<IEnumerable<object>> SearchRepositories(string repositoryName = null, string language = null, string username = null);
    }
}
