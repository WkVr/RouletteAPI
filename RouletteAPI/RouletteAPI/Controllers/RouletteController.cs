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

            await _sqLite.PlaceBet(betList);

            return Ok("Bets placed");
        }

        [HttpGet]
        [Route("Spin")]
        public async Task<int> GetSpin(int gameId)
        {
            Random rnd = new Random();

            int value = rnd.Next(0, 37);
            await _sqLite.Spin(new Spin
            {
                GameId = gameId,
                Value = value,
                Color = value == 0 ? "Green" : value / 2 == 0 ? "Black" : "Red",
            });

            return value;
        }

        [HttpGet]
        [Route("ShowPreviousSpins")]
        public async Task<IEnumerable<Spin>> GetGameSpin(int gameId)
        {
            return await _sqLite.GetSpinList(gameId);
        }
    }
}
