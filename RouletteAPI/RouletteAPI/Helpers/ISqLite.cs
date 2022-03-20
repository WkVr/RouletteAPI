using RouletteAPI.Models;

namespace RouletteAPI.Helpers
{
    public interface ISqLite
    {
        public Task<bool> PlaceBet(BetList betList);
        public Task<IEnumerable<Bet>> GetBetList(int gameId);
    }
}
