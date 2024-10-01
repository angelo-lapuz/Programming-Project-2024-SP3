using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeakHub.ViewModels;
using PeakHub.Models;
 
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;

namespace PeakHub.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");
        private readonly ILogger<LoginController> _logger;
        private readonly IEmailSender _emailSender;


        /// <summary>
        /// / sign in for like signing in - is auth, 2fa enabled 
        /// </summary>
        private readonly SignInManager<IdentityUser> _signInManager;


        // / <summary>
        // is the account confirmed, is it admin, is it a token account
        private readonly UserManager<IdentityUser> _userManager;

        public LoginController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IHttpClientFactory clientFactory, ILogger<LoginController> logger, IEmailSender emailSender)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Index() { return View(); }

        public IActionResult Login()  { return View(); }

        // checks if the username and password are correct changes the session to the user id and username
        // if the login is successful, if not an error message is added to the ModelState
        // and the view is returned.
        // POST: SignUp/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {


            if (!ModelState.IsValid) return View(viewModel);

            // calling API and encoding username and password to prevent capture in plain text
            var response = await Client.GetAsync($"api/users/{Uri.EscapeDataString(viewModel.UserName)}/{Uri.EscapeDataString(viewModel.Password)}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(result);

                if (user != null) {

                    if(!await _userManager.IsEmailConfirmedAsync(user))
                    {
                      ModelState.AddModelError("LoginError", "Email not confirmed");
                        return View(viewModel);
                    }



                    var currentUser = await _signInManager.PasswordSignInAsync(user.UserName, viewModel.Password, false, false);



                    // Store user details in session
                    HttpContext.Session.SetInt32("UserID", user.UserID);
                    HttpContext.Session.SetString("Username", user.UserName);
                    return RedirectToAction("Index", "Home");
                }
            }

            // If login fails, add error to the ModelState
            ModelState.AddModelError("LoginError", "Invalid login details");
            return View(viewModel);

        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


    }
}