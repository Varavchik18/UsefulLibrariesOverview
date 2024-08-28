using System.ComponentModel.DataAnnotations;

public class GitHubUser
{
    [Required(ErrorMessage = "Login is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Login must be between 3 and 50 characters.")]
    public string Login { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
    public string Name { get; set; }

    public string Company { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Followers count cannot be negative.")]
    public int Followers { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Following count cannot be negative.")]
    public int Following { get; set; }

    [Url(ErrorMessage = "Invalid URL format.")]
    public string AvatarUrl { get; set; }
}
