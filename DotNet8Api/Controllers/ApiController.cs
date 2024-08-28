using Microsoft.Extensions.Logging;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class GitHubController : ControllerBase
{
    private readonly IGitHubApi _gitHubApi;
    private readonly IValidator<GitHubUser> _validator;
    private readonly ILogger<GitHubController> _logger;

    public GitHubController(IGitHubApi gitHubApi, IValidator<GitHubUser> validator, ILogger<GitHubController> logger)
    {
        _gitHubApi = gitHubApi;
        _validator = validator;
        _logger = logger;
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetUser(string username)
    {
        try
        {
            _logger.LogInformation("Starting request to get user data for {Username}", username);

            var user = await _gitHubApi.GetUserAsync(username);

            var validationResult = _validator.Validate(user);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for user {Username} with errors: {Errors}", username, validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }

            _logger.LogInformation("Successfully retrieved and validated user data for {Username}", username);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request for user {Username}", username);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
