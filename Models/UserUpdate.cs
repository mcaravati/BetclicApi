namespace BetclicApi.Models;


/// <summary>
/// Data structure used in a /api/User/{id} PUT request, updates a user's points from its ID.
/// </summary>
public class UserUpdate
{
    /// <summary>
    /// The amount of points the user should have once updated.
    /// </summary>
    public int Points { get; set; } = 0;
}