using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        private int? UserID => HttpContext.Session.GetInt32("UserID");
        private string Name => HttpContext.Session.GetString("Username");

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }




        public async Task<IActionResult> Index()
        {
            HomeViewModel viewModel = new HomeViewModel();
            if (UserID == null)
            {
                viewModel.UserID = null;
                viewModel.UserName = null;
            }
            else
            {
                var response = await Client.GetAsync($"api/users/{UserID}");

                if (!response.IsSuccessStatusCode)
                    throw new Exception();

                var result = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(result);

                viewModel.UserID = user.UserID;
                viewModel.UserName = user.UserName;
            }


            return View(viewModel);
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
