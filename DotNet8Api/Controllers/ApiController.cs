using FluentValidation;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class GitHubController : ControllerBase
{
    private readonly IGitHubApi _gitHubApi;
    private readonly IValidator<GitHubUser> _validator;

    public GitHubController(IGitHubApi gitHubApi, IValidator<GitHubUser> validator)
    {
        _gitHubApi = gitHubApi;
        _validator = validator;
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetUser(string username)
    {
        var user = await _gitHubApi.GetUserAsync(username);

        var validationResult = _validator.Validate(user);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        return Ok(user);
    }
}
