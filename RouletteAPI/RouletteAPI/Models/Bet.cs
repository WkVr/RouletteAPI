using RouletteAPI.Enum;

namespace RouletteAPI.Models
{   
    public class Bet
    {
        public BetType BetType { get; set; }
        public int BetAmount { get; set; }
    }
}
