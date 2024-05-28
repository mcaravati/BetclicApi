using System.ComponentModel.DataAnnotations;

namespace BetclicApi.Models;

/// <summary>
/// Data structure used in a /api/User POST request, creates a user from a username.
/// </summary>
public class UserCreation
{
    /// <summary>
    /// The username of the user. The username must be between 3 and 30 characters.
    /// </summary>
    [Required]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 30 characters.")]
    public string Username { get; set; } = string.Empty;
}