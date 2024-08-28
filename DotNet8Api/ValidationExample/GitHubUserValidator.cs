using FluentValidation;

public class GitHubUserValidator : AbstractValidator<GitHubUser>
{
    public GitHubUserValidator()
    {
        RuleFor(user => user.Login)
            .NotEmpty().WithMessage("Login is required.")
            .Length(3, 50).WithMessage("Login must be between 3 and 50 characters.");

        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(0, 100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(user => user.Company)
            .MaximumLength(100).WithMessage("Company name must not exceed 100 characters.");

        RuleFor(user => user.Followers)
            .GreaterThanOrEqualTo(0).WithMessage("Followers count cannot be negative.");

        RuleFor(user => user.Following)
            .GreaterThanOrEqualTo(0).WithMessage("Following count cannot be negative.");

        RuleFor(user => user.AvatarUrl)
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            .WithMessage("Invalid URL format for AvatarUrl.");
    }
}
