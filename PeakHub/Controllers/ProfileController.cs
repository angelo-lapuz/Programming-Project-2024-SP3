using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeakHub.Models;
using PeakHub.ViewModels;
using System.Text;

namespace PeakHub.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ProfileController> _logger;
        private readonly string defaultImg = "https://peakhub-user-content.s3.amazonaws.com/default.jpg";
        private HttpClient _httpClient => _clientFactory.CreateClient("api");


        public ProfileController(IHttpClientFactory clientFactory, ILogger<ProfileController> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;

        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/users/GetUser");
            var content = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(content);

            if (user == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string defaultImg = "https://peakhub-user-content.s3.amazonaws.com/default.jpg";
            string profileImg = !string.IsNullOrEmpty(user.ProfileIMG) ? user.ProfileIMG : defaultImg;

            var profileViewModel = new ProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                TotalCompleted = user.UserPeaks.Count,
                Peaks = user.UserPeaks.Select(up => up.Peak).ToList(),
                Awards = user.UserAwards.Select(ua => ua.Award).ToList(),
                ProfileImg = profileImg
            };

            ViewBag.UserName = profileViewModel.UserName;
            ViewBag.Peaks = profileViewModel.Peaks;
            ViewBag.Awards = profileViewModel.Awards;
            ViewBag.ProfileImg = profileViewModel.ProfileImg;

            return View(profileViewModel);
        }


        public async Task<IActionResult> EditDetails()
        {
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

                _logger.LogInformation("success");
                return View(user);
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
            var response = await _httpClient.GetAsync($"api/users/VerifyPassword/{model.OldPassword}");

            //var resultContent = await response.Content.ReadAsStringAsync();
            //dynamic result = JsonConvert.DeserializeObject(resultContent);

            if (response != null)
            {
                _logger.LogInformation(response.ToString());
                // convert viewmodel to json to be sent to api/users/ChangePassword
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                response = await _httpClient.PostAsync("api/users/ChangePassword", content);

                if (!response.IsSuccessStatusCode)
                {

                    var errorMessage = await response.Content.ReadAsStringAsync();
                    _logger.LogError(errorMessage);
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
                ViewBag.PasswordChangeStatus = "Password changed successfully.";
                return View(model);
            }
            else
            {
                ModelState.AddModelError("OldPassword", "Failed to change password");
            }

            return View(model);
        }

    }
}
