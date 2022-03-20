using Microsoft.AspNetCore.Mvc;
using System.Web.Http;

namespace RouletteAPI.Controllers
{
    public class RouletteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
