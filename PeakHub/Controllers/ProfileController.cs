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
        private readonly ILogger<ProfileController> _logger;
        private HttpClient _httpClient => _clientFactory.CreateClient("api");


        public ProfileController(IHttpClientFactory clientFactory, ILogger<ProfileController> logger)

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
            ViewBag.Peaks = user.UserPeaks.Select(up => up.Peak).ToList();
            ViewBag.Awards = user.UserAwards;

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

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            ViewBag.PasswordChangeStatus = null;
            return View();
        }

        // changes password given by user
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var response = await _httpClient.GetAsync($"api/users/VerifyPassword/{model.OldPassword}");
            var resultContent = await response.Content.ReadAsStringAsync();

            dynamic result = JsonConvert.DeserializeObject(resultContent);

            bool passwordsMatch = result.PasswordMatch;

            if (!passwordsMatch) ModelState.AddModelError("OldPassword", "Username already exists");

            if (ModelState.IsValid)
            {
                // convert viewmodel to json to be sent to api/users/ChangePassword
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                response = await _httpClient.PostAsync("api/users/ChangePassword", content);

                // return to profile page if changed successfully
                if (response.IsSuccessStatusCode)
                {
                    ViewBag.PasswordChangeStatus = "Password changed successfully.";
                    return View(model);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    _logger.LogError(errorMessage);
                    ModelState.AddModelError(string.Empty, errorMessage);
                }

            }
            return View(model);
        }
    }
}
