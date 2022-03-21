using RouletteAPI.Models;

namespace RouletteAPI.Helpers
{
    public interface ISqLite
    {
        public Task<int> PlaceBet(BetList betList);
        public Task<IEnumerable<Bet>> GetBetList(int gameId);
        public Task Spin(Spin spin);
        public Task<IEnumerable<Spin>> GetSpinList(int gameId);
        public Task<int> GetTableBet(int gameId);

    }
}
