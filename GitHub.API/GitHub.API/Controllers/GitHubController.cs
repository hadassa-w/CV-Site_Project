using GitHub;
using GitHub.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GitHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class GitHubController : Controller
    {
        private readonly IGitHubService _githubService;
        private readonly GitHubIntegrationOptions _integrationOptions;

        public GitHubController(IGitHubService githubService, IOptions<GitHubIntegrationOptions> options)
        {
            _githubService = githubService;
            _integrationOptions = options.Value;
        }

        [HttpGet("repositories/{username}")]
        public async Task<IActionResult> GetRepos(string username)
        {
            var repos = await _githubService.GetUserRepositories(username);
            return Ok(repos.Select(r => new
            {
                r.Name,
                r.Description,
                r.Language,
                r.StargazersCount,
                r.UpdatedAt,
                r.HtmlUrl
            }));
        }

        [HttpGet("portfolio")]
        public async Task<IActionResult> GetPortfolio()
        {
            var portfolio = await _githubService.GetPortfolio();
            return Ok(portfolio);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchRepositories([FromQuery] string? repositoryName = null, [FromQuery] string? language = null, [FromQuery] string? username = null)
        {
            var results = await _githubService.SearchRepositories(repositoryName, language, username);
            return Ok(results);
        }

    }
}
