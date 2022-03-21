using Microsoft.AspNetCore.Mvc;
using RouletteAPI.Helpers;
using RouletteAPI.Models;

namespace RouletteAPI.Controllers
{
    public class RouletteController : Controller
    {
        private readonly ISqLite _sqLite;

        public RouletteController(ISqLite sqLite) => _sqLite = sqLite;

        [HttpGet]
        [Route("GetBets")]
        public async Task<IEnumerable<Bet>> GetBets(int gameId) => await _sqLite.GetBetList(gameId);

        [HttpPost]
        [Route("PlaceBets")]
        public async Task<IActionResult> PostBets([FromBody] BetList betList)
        {
            if (betList == null)
                return BadRequest("No Bets Selected");

            var betId = await _sqLite.PlaceBet(betList);

            return betId != 0? Ok("Bets placed. BetID: " + betId): BadRequest();
        }

        [HttpGet]
        [Route("Spin")]
        public async Task<int> GetSpin(SpinRequest spin)
        {
            Random rnd = new Random();

            int value = rnd.Next(0, 37);
            await _sqLite.Spin(new Spin
            {
                GameId = spin.GameId,
                Value = value,
                Color = value == 0 ? "Green" : value / 2 == 0 ? "Black" : "Red",
                BetId = spin.BetId,
            });

            return value;
        }

        [HttpGet]
        [Route("ShowPreviousSpins")]
        public async Task<IEnumerable<Spin>> GetGameSpin(int gameId)
        {
            return await _sqLite.GetSpinList(gameId);
        }

        [HttpGet]
        [Route("Payout")]
        public async Task<int> Payout(int gameId)
        {
            var totalPayout = 0;

            var tableBet = await _sqLite.GetTableBet(gameId);

            var spins = await _sqLite.GetSpinList(gameId);

            var bets = await _sqLite.GetBetList(gameId);

            var results = bets.Join(spins,
                bet => bet.BetId,
                spin => spin.BetId,
                (bet, spin) =>
                {
                    return new BetOutcome
                    {
                        SpinValue = spin.Value,
                        SpinColor = spin.Color,
                        BetValues = bet.Values?.Split(",").ToList() ?? new(),
                        Payout = GetPayout((int)bet.BetType),
                        TableBet = tableBet,
                        BetAmount = bet.BetAmount,
                    };
                }
            ).ToList();

            foreach(var x in results)
            {
                if (x.BetValues.Contains(x.SpinValue.ToString()))
                    totalPayout += x.Payout * (x.TableBet * x.BetAmount);
            }

            return totalPayout;
        }

        public int GetPayout(int betType)
        {
            switch (betType)
            {
                case 0: return 35;
                case 1: return 17;
                case 2: return 11;
                case 3: return 8;
                case 4: return 5;
                case 5: return 2;
                case 6: return 2;
                case 7: return 1;
                case 8: return 1;
                case 9: return 1;
                default: return 0;
            }
        }
    }

    public class SpinRequest
    {
        public int GameId { get; set; }
        public int BetId { get; set; }
    }

    public class BetOutcome
    {
        public int SpinValue { get; set; }
        public string SpinColor { get; set; } = null!;
        public List<string> BetValues { get; set; } = null!;
        public int Payout { get; set; }
        public int TableBet { get; set; }
        public int BetAmount { get; set; }
    }
}
