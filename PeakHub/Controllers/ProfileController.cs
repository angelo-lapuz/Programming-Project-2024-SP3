using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeakHub.ViewModels;
using PeakHub.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Text;

namespace PeakHub.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<HomeController> _logger;
        private HttpClient _httpClient => _clientFactory.CreateClient("api");

        public ProfileController(IHttpClientFactory clientFactory, ILogger<HomeController> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // gets user object from db
            var response = await _httpClient.GetAsync($"api/users/GetUser");
            var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());

            ViewBag.UserName = user.UserName;
            ViewBag.Peaks = user.Peaks;
            ViewBag.Awards = user.Awards;

            return View();
        }

        public async Task<IActionResult> EditDetails()
        {
            // gets user object from db
            var response = await _httpClient.GetAsync($"api/users/GetUser");
            var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());

            return View(user);
        }

        //[HttpPost]
        //public async Task<IActionResult> EditDetails(EditDetailsViewModel model)
        //{
        //    // gets user object from db
        //    var response = await _httpClient.GetAsync($"api/users/GetUser");
        //    var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());

        //    user.FirstName = model.FirstName;
        //    user.LastName = model.LastName;
        //    user.Address = model.Address;
        //    //user.PhoneNumber = model.PhoneNumber;

        //    var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
        //    var updatedUser = await _httpClient.PutAsJsonAsync($"api/users/UpdateUser", content);
        //    return View(user);
        //}

        [HttpPost]
        public async Task<IActionResult> EditDetails(EditDetailsViewModel model)
        {
            // gets user object from db

            _logger.LogInformation("Sending login request to API.");

            var response = await _httpClient.GetAsync($"api/users/GetUser");
            var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            //user.PhoneNumber = model.PhoneNumber;

            _logger.LogInformation("user = " + user);
            var result = await _httpClient.PostAsJsonAsync("api/users/UpdateUser", user);

            _logger.LogInformation(result.ToString());
            if (result.IsSuccessStatusCode)
            {
                // return on success // do other stuff
                _logger.LogInformation("success");
                return View(user);
            }
            else
            {
                // return ? on fail // do other stuff - maybe Viewmodel errors
                _logger.LogInformation("failed");
                return BadRequest(new { error = "Failed to update user with new route." });
            }
        }

    }
}
