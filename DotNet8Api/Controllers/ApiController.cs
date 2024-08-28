using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class GitHubController : ControllerBase
{
    private readonly IGitHubApi _gitHubApi;

    public GitHubController(IGitHubApi gitHubApi)
    {
        _gitHubApi = gitHubApi;
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetUser(string username)
    {
        var user = await _gitHubApi.GetUserAsync(username);
        return Ok(user);
    }
}
