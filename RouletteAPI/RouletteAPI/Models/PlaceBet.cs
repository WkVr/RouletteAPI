using RouletteAPI.Enum;

namespace RouletteAPI.Models
{
    public class PlaceBet
    {
        List<Bet> Bets { get; set; } = null!;
        int TableBet { get; set; }
    }
}
