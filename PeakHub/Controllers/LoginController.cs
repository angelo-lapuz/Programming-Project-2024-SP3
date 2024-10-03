using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeakHub.ViewModels;
using PeakHub.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;



namespace PeakHub.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<LoginController> _logger;
        private readonly HttpClient _httpClient;

        public LoginController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<LoginController> logger,
            IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("api");
        }

        public IActionResult Index() { return View(); }

        public IActionResult Login() { return View(); }

        // checks if the username and password are correct changes the session to the user id and username
        // if the login is successful, if not an error message is added to the ModelState
        // and the view is returned.
        // POST: SignUp/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountViewModel model)
        {
            var loginModel = model.LoginModel;

            if (!ModelState.IsValid) return View(model);

            _logger.LogInformation("Sending login request to API.");

            // Call the API to verify credentials and log in the user
            var response = await _httpClient.PostAsJsonAsync("api/users/login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(result);

                // Store user details in session (if needed)
                HttpContext.Session.SetString("UserID", user.Id);
                HttpContext.Session.SetString("Username", user.UserName);

                return RedirectToAction("Index", "Home");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ModelState.AddModelError("LoginModel.UserName", "Please confirm your email to sign in.");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                ModelState.AddModelError("LoginModel.UserName", "Invalid username or password.");
            }
            else
            {
                ModelState.AddModelError("LoginModel.UserName", "An unexpected error occurred, please try again.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            var response = await _httpClient.PostAsync("api/users/logout", null);
            if (response.IsSuccessStatusCode)
            {
                HttpContext.Session.Clear();  
                _logger.LogInformation("User logged out successfully.");
            }
            return RedirectToAction("Index", "Home");
        }


    }
}