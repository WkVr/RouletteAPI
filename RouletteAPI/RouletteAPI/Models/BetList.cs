using RouletteAPI.Enum;

namespace RouletteAPI.Models
{
    public class BetList
    {
        public int? GameId { get; set; }
        public int TableBet { get; set; }
        public List<Bet> Bets { get; set; } = null!;
    }
}
