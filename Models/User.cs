namespace BetclicApi.Models;

public class User
{
    public long Id { get; set; }
    public string NickName { get; set; } = string.Empty;
    public int Points { get; set; } = 0;
    public uint Rank { get; set; } = 0;
}