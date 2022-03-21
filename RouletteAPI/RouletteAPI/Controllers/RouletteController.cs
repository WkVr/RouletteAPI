using Microsoft.AspNetCore.Mvc;
using RouletteAPI.Helpers;
using RouletteAPI.Models;

namespace RouletteAPI.Controllers
{
    public class RouletteController : Controller
    {
        private readonly ISqLite _sqLite;
        private GameSpin gameSpin;

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
        public async Task<int> GetSpin()
        {
            Random rnd = new Random();

            int value = rnd.Next(1, 37);
            await gameSpin.UpdateSpin(value);

            return value;
        }

        [HttpGet]
        [Route("ShowPreviousSpins")]
        public async Task<List<Spin>> GetGameSpin()
        {
            return await gameSpin.GetPreviousSpins();
        }
    }
}
