using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeakHub.Models;
using PeakHub.Utilities;
using PeakHub.ViewModels;
using SimpleHashing.Net;
using System.Text;

namespace PeakHub.Controllers {
	public class AccountController : Controller {
		// -------------------------------------------------------------------------------- //
		private readonly IHttpClientFactory _clientFactory;
		private HttpClient Client => _clientFactory.CreateClient("api");
		public AccountController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;
		// -------------------------------------------------------------------------------- //
		public IActionResult Login() {
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Account_Login(LoginViewModel viewModel) {
			WebAPIUtilities utilities = new(_clientFactory);

			if (ModelState.IsValid) {
                User user = await utilities.GetUser(viewModel.UserName);
                if (user == null) ModelState.AddModelError("LoginError", "Invalid Username. Does Not Exist");

                else {
                    SimpleHash simpleHash = new();

                    if (!simpleHash.Verify(viewModel.Password, user.Password)) 
                        ModelState.AddModelError("LoginError", "Invalid Password.");

                    else {
                        HttpContext.Session.SetInt32("UserID", user.UserID);
                        HttpContext.Session.SetString("Username", user.UserName);

                        return RedirectToAction("Dashboard", "Home");
                    }
                }
			}

            return View("Login", viewModel);
        }
		// -------------------------------------------------------------------------------- //
		public IActionResult Signup() {
			return View();
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Account_Register(SignUpViewModel viewModel) {
            WebAPIUtilities utilities = new(_clientFactory);

            if (ModelState.IsValid) {
                if (await utilities.VerifyUsername(viewModel.UserName)) {
                    ModelState.AddModelError("UserName", "Username Already Exists.");
                }
                if (await utilities.VerifyEmail(viewModel.Email)) {
                    ModelState.AddModelError("Email", "Email Is In Use.");
                }
                if (viewModel.Password != viewModel.ConfirmPassword) {
                    ModelState.AddModelError("ConfirmPassword", "Passwords Do Not Match!");
                }

                // Second Check - For Existing Attributes
                if (ModelState.IsValid) {
                    SimpleHash simpleHash = new();
                    string hashedPassword = simpleHash.Compute(viewModel.Password);

                    var user = new User {
                        UserName = viewModel.UserName,
                        Email = viewModel.Email,
                        Password = hashedPassword,
                    };

                    var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    var response = await Client.PostAsync("api/users", content);

                    if (response.IsSuccessStatusCode) {
                        return RedirectToAction("Dashboard", "Home");
                    }
                }
            }

            return View("Signup", viewModel);
        }
        // -------------------------------------------------------------------------------- //
    }
}
