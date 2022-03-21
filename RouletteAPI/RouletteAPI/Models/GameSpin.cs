namespace RouletteAPI.Models
{
    public class GameSpin
    {
        List<Spin> spins = new List<Spin>();

        public GameSpin()
        {
            var spin = new Spin();

            for (int i = 0; i < 37; i++)
            {
                spins.Add(
                    new Spin
                    {
                        Value = i,
                        Color = i / 2 == 0 ? "Black" : "Red",
                        IsSpun = false,
                    });
            }
        }

        public Task UpdateSpin(int value)
        {
            var spin = spins.FirstOrDefault(x => x.Value == value);
            if(spin != null)
                spin.IsSpun = true;

            return Task.CompletedTask;
        }
    }

    public class Spin
    {
        public int? GameId { get; set; }
        public int Value { get; set; }
        public string Color { get; set; }
        public bool IsSpun { get; set; }
    }
}
