using System.Data.Entity;

namespace RouletteAPI.Models
{
    public class Spin
    {
        public int SpinId { get; set; }
        public int? GameId { get; set; }
        public int Value { get; set; }
        public string Color { get; set; } = null!;
        public int BetId { get; set; }
    }
}
