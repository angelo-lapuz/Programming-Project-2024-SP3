using Microsoft.AspNetCore.Mvc;
using SimpleHashing.Net;
using Newtonsoft.Json;
using PeakHub.ViewModels;
using PeakHub.Models;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace PeakHub.Controllers {
    public class SignUpController : Controller {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private HttpClient Client => _clientFactory.CreateClient("api");

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
            var response = await Client.GetAsync($"api/users/Verify/{viewModel.UserName}/{viewModel.Email}");
            var resultContent = await response.Content.ReadAsStringAsync();

            dynamic result = JsonConvert.DeserializeObject(resultContent);

            if ((bool)result.UsernameExists) ModelState.AddModelError("UserName", "Username exists");
            if ((bool)result.EmailExists) ModelState.AddModelError("Email", "Email already in use");

            if (ModelState.IsValid) {
                // convert viewmodel to json to be sent to api/users/create
                var content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");
                response = await Client.PostAsync("api/users/Create", content);

                // return to login page if created successfully
                if (response.IsSuccessStatusCode) 
                {
                 
                    return RedirectToAction("Login", "Login");
                } 
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    _logger.LogError(errorMessage);
                    ModelState.AddModelError(string.Empty,"Signup failed, try again.");
                }
            }

            return View(viewModel);
        }




    }
}
