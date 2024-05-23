using System.ComponentModel.DataAnnotations;

namespace BetclicApi.Models;

public class UserCreation
{
    [Required]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "NickName must be between 3 and 30 characters.")]
    public string NickName { get; set; } = string.Empty;
}