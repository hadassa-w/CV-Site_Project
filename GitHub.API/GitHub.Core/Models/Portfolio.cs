using Octokit;

namespace GitHub.API.Models
{
    public class Portfolio
    {
        public string UserName { get; set; }
        public List<Repository> Repositories { get; set; }
    }
}
