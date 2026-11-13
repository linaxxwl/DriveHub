using System.Diagnostics;
using DriveHub.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace DriveHub.Web.Controllers
{
    /// <summary>Страница «О проекте» с темой работы и исполнителем.</summary>
    public class HomeController : Controller
    {

        public IActionResult About() => View();

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
