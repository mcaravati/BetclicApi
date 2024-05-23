using System.ComponentModel.DataAnnotations;

namespace BetclicApi.Models;

public class User
{
    public long Id { get; set; }
    
    [Required]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "NickName must be between 3 and 30 characters.")]
    public string NickName { get; set; } = string.Empty;
    public int Points { get; set; }
    public uint Rank { get; set; } // Will be computed on-request
}