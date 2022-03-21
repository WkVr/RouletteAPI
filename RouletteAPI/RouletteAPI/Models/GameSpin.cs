using System.Data.Entity;

namespace RouletteAPI.Models
{
    public class GameSpin
    {
        public List<Spin> Spins { get; set; } = null!;

        public GameSpin()
        {
            if(Spins == null)
            {
                Spins = new List<Spin>();

                for (int i = 0; i < 37; i++)
                {
                    Spins.Add(
                        new Spin
                        {
                            Value = i,
                            Color = i == 0 ? "Green" : i / 2 == 0 ? "Black" : "Red",
                            IsSpun = false,
                        });
                }
            }
        }

        public Task UpdateSpin(int value)
        {
            var spin = Spins.ToList().FirstOrDefault(x => x.Value == value);
            if (spin != null)
                spin.IsSpun = true;

            return Task.CompletedTask;
        }

        public async Task<List<Spin>> GetPreviousSpins() => Spins.Where(s => s.IsSpun).ToList();
    }

    public class Spin
    {
        public int? GameId { get; set; }
        public int Value { get; set; }
        public string Color { get; set; }
        public bool IsSpun { get; set; }
    }
}
