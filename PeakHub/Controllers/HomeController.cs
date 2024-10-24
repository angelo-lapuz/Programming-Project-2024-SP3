using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeakHub.Models;
using System.Diagnostics;
using PeakHub.ViewModels;

namespace PeakHub.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;

        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory) {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index() {
            HttpContext.Session.SetString("LastPage", "Home");
            return View(new HomeViewModel());
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
