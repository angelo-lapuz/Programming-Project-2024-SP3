using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeakHub.ViewModels;
using PeakHub.Models;
using System.Text;
using PeakHub.Utilities;

namespace PeakHub.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ProfileController> _logger;
        private readonly Tools _tools;
        private HttpClient _httpClient => _clientFactory.CreateClient("api");
        private string userID => HttpContext.Session.GetString("UserId");



        public ProfileController(IHttpClientFactory clientFactory, ILogger<ProfileController> logger, Tools tools)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _tools =  tools;

        }

        public async Task<IActionResult> Index()
        {
            // gets user object from db
            if(userID == null)
            {
                return RedirectToAction("Login", "Login");
            }
            User user = await _tools.GetUser(userID);

            // get awards
            var awards = await _httpClient.GetAsync("api/awards");

            string defaultImg = "https://peakhub-user-content.s3.amazonaws.com/default.jpg";
            string profileImg = !string.IsNullOrEmpty(user.ProfileIMG) ? user.ProfileIMG : defaultImg;

            ViewBag.UserName = user.UserName;
            ViewBag.Email = user.Email;
            ViewBag.Peaks = user.UserPeaks.Select(up => up.Peak).ToList();
            ViewBag.Awards = user.UserAwards.Select(ua => ua.Award).ToList();
            ViewBag.ProfileImg = profileImg;
            ViewBag.totalCompleted = user.UserPeaks.Count;
            ViewBag.AllAwards = awards;

            return View();
        }

        public async Task<IActionResult> EditDetails()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditDetails(EditDetailsViewModel model)
        {

            User user = await _tools.GetUser(userID);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;

            _logger.LogInformation("user = " + user);
            var result = await _httpClient.PostAsJsonAsync("api/users/UpdateUser", user);

            _logger.LogInformation(result.ToString());
            if (result.IsSuccessStatusCode)
            {

                _logger.LogInformation("success");
                
                return View(model);
            }
            else
            {
                _logger.LogInformation("failed");
                return BadRequest(new { error = "Failed to update user with new route." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            ViewBag.PasswordChangeStatus = null;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.ID = userID; ;

            // convert viewmodel to json to be sent to api/users/ChangePassword
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/users/ChangePassword", content);

            if (!response.IsSuccessStatusCode)
            {

                var errorMessage = await response.Content.ReadAsStringAsync();
                _logger.LogError(errorMessage);
                ModelState.AddModelError(string.Empty, errorMessage);
            }
            else
            {
                ModelState.AddModelError("OldPassword", "Failed to change password");

            }
            ViewBag.PasswordChangeStatus = "Password changed successfully.";
            return View(model);

        }

    }
}
