namespace BetclicApi.Services;

using Models;

public class RankingService
{
    private readonly BetclicContext _dbContext;

    public RankingService(BetclicContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task UpdateRanks()
    {
        var users = _dbContext.User.ToList();
        
        users.Sort((x, y) => y.Points.CompareTo(x.Points));
        for (var i = 0; i < users.Count; i++)
        {
            users[i].Rank = (uint) i + 1;
        }

        foreach (var user in users)
        {
            _dbContext.User.Update(user);
        }

        await _dbContext.SaveChangesAsync();
    }
}