using Microsoft.AspNetCore.Mvc;
using SimpleHashing.Net;
using Newtonsoft.Json;
using PeakHub.ViewModels;
using PeakHub.Models;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;

namespace PeakHub.Controllers {
    public class SignUpController : Controller {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private HttpClient _httpClient => _clientFactory.CreateClient("api");

        public SignUpController(IHttpClientFactory clientFactory, ILogger<HomeController> logger) {
            _clientFactory = clientFactory;
            _logger = logger;
        }
        public IActionResult Index() { return View(); }
        public IActionResult Create() { return View(); }

        // Takes input given by user from form, verifies is username and email
        // aren't in the database, if one or both are found in the database,
        // a error message is added to the ModelState which is then returned back
        // to the Creat view. If there email and usenrame arent found and there are
        // no errors, SimpleHashing is used to hash the password and a user is
        // created.
        // POST: SignUp/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SignUpViewModel viewModel) {

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var response = await _httpClient.GetAsync($"api/users/Verify/{viewModel.UserName}/{viewModel.Email}");
            var resultContent = await response.Content.ReadAsStringAsync();

            dynamic result = JsonConvert.DeserializeObject(resultContent);

            bool usernameExists = result.usernameExists != null ? (bool)result.usernameExists : false;
            bool emailExists = result.emailExists != null ? (bool)result.emailExists : false;

            if (usernameExists) ModelState.AddModelError("UserName", "Username already exists");
            if (emailExists) ModelState.AddModelError("Email", "Email already in use");

            if (ModelState.IsValid) {
                // convert viewmodel to json to be sent to api/users/create
                var content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");
                response = await _httpClient.PostAsync("api/users/Create", content);

                // return to login page if created successfully
                if (response.IsSuccessStatusCode) 
                {
                    return RedirectToAction("Login", "Login");
                } 
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    _logger.LogError(errorMessage);
                    ModelState.AddModelError(string.Empty,errorMessage);
                }
            }

            return View(viewModel);
        }

       // displays ConfirmEmail view rather than havethis done through the API
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            // calling api
            var response = await _httpClient.GetAsync($"api/users/ConfirmEmail/{userId}/{Uri.EscapeDataString(token)}");
            
            var viewModel = new EmailConfirmationViewModel();

            // confirmed email address
            if (response.IsSuccessStatusCode)
            {
                viewModel.IsSuccess = true;
            }
            // failed to confirm email address
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                viewModel.IsSuccess = false;
                viewModel.ErrorMessage = errorMessage;
            }

            return View("Confirm", viewModel);
        }

        public IActionResult ConfirmEmailSuccess()
        {
            return View();
        }

        public IActionResult ConfirmEmailFailed(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            return View();
        }
    }

}
