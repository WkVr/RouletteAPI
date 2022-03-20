using Microsoft.AspNetCore.Mvc;
using RouletteAPI.Helpers;
using RouletteAPI.Models;
using System.Net.Http;

namespace RouletteAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BetController : Controller
    {

        private readonly ISqLite _sqLite;

        public BetController(ISqLite sqLite) => _sqLite = sqLite;

        [HttpGet]
        [Route("GetBets")]
        public async Task<IEnumerable<Bet>> Get(int gameId) => await _sqLite.GetBetList(gameId);

        [HttpPost]
        [Route("PlaceBets")]
        public async Task<IActionResult> Post([FromBody] BetList betList)
        {
            if (betList == null)
                return BadRequest("No Bets Selected");

            await _sqLite.PlaceBet(betList);

            return Ok("Bets placed");
        }
    }
}
