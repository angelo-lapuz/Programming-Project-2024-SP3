using Microsoft.AspNetCore.Mvc;
using PeakHub.Models;
using System.Diagnostics;
using PeakHub.ViewModels;

namespace PeakHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");


        public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        // display index
        public async Task<IActionResult> Index()
        {
            HomeViewModel viewModel = new HomeViewModel();

            return View(viewModel);
        }

        //display privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        // displays errror page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
