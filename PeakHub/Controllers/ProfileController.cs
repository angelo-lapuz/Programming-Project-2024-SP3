using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeakHub.ViewModels;
using PeakHub.Models;
using System.Text;
using PeakHub.Utilities;

namespace PeakHub.Controllers {
    public class ProfileController : Controller {
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

        // Index page for user profile - displays user details, peaks and awards
        public async Task<IActionResult> Index()
        {
            // ensures that the user is currently logged in
            if(userID == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // get the current user
            User user = await _tools.GetUser(userID);

            // set the image for the user profile
            string defaultImg = "https://peakhub-user-content.s3.amazonaws.com/default.jpg";
            string profileImg = !string.IsNullOrEmpty(user.ProfileIMG) ? user.ProfileIMG : defaultImg;

            // put the user details, peaks and awards into the viewbag
            ViewBag.UserName = user.UserName;
            ViewBag.Email = user.Email;
            ViewBag.Peaks = user.UserPeaks.Select(up => up.Peak).ToList();
            ViewBag.Awards = user.UserAwards.Select(ua => ua.Award).ToList();
            ViewBag.ProfileImg = profileImg;
            ViewBag.totalCompleted = user.UserPeaks.Count;

            return View();
        }

        // returns the EditDetails page for the user - allows the user to edit their details
        [HttpGet]
        public async Task<IActionResult> EditDetails()
        {
            return View();
        }

        // called when the user is attempting to edit their details from the front
        [HttpPost]
        public async Task<IActionResult> EditDetails(EditDetailsViewModel model)
        {

            // get the current user
            User user = await _tools.GetUser(userID);

            // update the user details with the new details - call api to update the user
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            var result = await _httpClient.PutAsJsonAsync("api/users", user);

            // if the user was updated successfully, return the view else return an error
            if (result.IsSuccessStatusCode)
            {                
                return View(model);
            }
            else
            {
                return BadRequest(new { error = "Failed to update user with new route." });
            }
        }

        // returns the ChangePassword page for the user - allows the user to change their password
        [HttpGet]
        public async Task<IActionResult> ChangePassword() {
            ViewBag.PasswordChangeStatus = null;
            return View();
        }

        // called when the user is attempting to change their password from the front
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            // check if the model is valid
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.ID = userID; 

            // convert viewmodel to json to be sent to api/users/ChangePassword
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/users/ChangePassword", content);

            // if password failed to change display error message
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, errorMessage);
            }

            else
            {
                _logger.LogInformation("here");
                ModelState.AddModelError("OldPassword", "Failed to change password");
            }
            // display Password changed successfully message
            ViewBag.PasswordChangeStatus = "Password changed successfully.";
            return View(model);
        }

    }
}
