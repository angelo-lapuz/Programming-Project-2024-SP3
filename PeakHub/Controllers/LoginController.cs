using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeakHub.ViewModels;
using PeakHub.Models;
using Microsoft.AspNetCore.Identity;
using PeakHub.Utilities;



namespace PeakHub.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<WebAPI.Models.User> _userManager;
        private readonly ILogger<LoginController> _logger;
        private readonly HttpClient _httpClient;
        private readonly Tools _tools;

        public LoginController(
            UserManager<WebAPI.Models.User> userManager,
            ILogger<LoginController> logger,
            IHttpClientFactory httpClientFactory,
            Tools tools)
        {
            _tools = tools;
            _userManager = userManager;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("api");
        }

        // displays the login page
        public IActionResult Login() { return View(); }

        // checks if the username and password are correct changes the session to the user id and username
        // if the login is successful, if not an error message is added to the ModelState
        // and the view is returned.
        // POST: SignUp/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountViewModel model) {
            var loginModel = model.LoginModel;

            // if the model state is not valid - return to the login
            if (!ModelState.IsValid) return View(model);

            //attempt to log the user in through the api
            var response = await _httpClient.PostAsJsonAsync("api/users/login", loginModel);

            // if user loggged in successfully then assign values and re-direct the user to the Home
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(result);

                HttpContext.Session.SetString("UserId", user.Id);
                return Redirection();
            }

            // user has not confirmed email
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                ModelState.AddModelError("LoginModel.UserName", "Please confirm your email to sign in.");
            }

            // invalid username or password
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) {
                ModelState.AddModelError("LoginModel.UserName", "Invalid username or password.");
            }

            // unexpected error could be various things, no api, db error, etc.
            else {
                ModelState.AddModelError("LoginModel.UserName", "An unexpected error occurred, please try again.");
            }

            return View(model);
        }

        // Reset Password function is called when the user clicks on a link that has been sent to their email
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId, string token) {

            // ensure that there is a userId and a token present with the request to the page if not return to the login page
            if(userId == null || token == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // creating new ResetPasswordViewModel and assigning values to it
            var model = new ResetPasswordViewModel
            {
                UserId = userId,
                Token = token
            };

            // displaying the resetPassword view to the user
            return View();
        }

        // called when the user submits to reset their password on the resetpassword Page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            // if viewmodel is invalid return the user to the page and display errors
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // call api to reset the users password with a new one
            var response = await _httpClient.PostAsJsonAsync("api/users/resetpassword", model);

            // if password changed successfully - redirect to the confirmation page to let them know
            // their password has been changed
            if (response.IsSuccessStatusCode)
            {
                 return View("Confirm");
            }

            return View(model);
        }


        // forgotPassword used to recover the users password - API will send a link to the users email for reset
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(AccountViewModel model)
        {
            // setting the appropriate viewModel
            var forgotPasswordModel = model.ForgotPasswordModel;

            // returning if model is not validd
            if (!ModelState.IsValid || forgotPasswordModel == null)
            {
                return View("Login", model);
            }

            // calling the api to attempt to reset the users password.
            var response = await _httpClient.PostAsJsonAsync("api/users/forgotpassword", forgotPasswordModel);

            // take user to page confirming the email has been sent
            if (response.IsSuccessStatusCode)
            {
                return View("ConfirmResetPassword");
            }
            // will occur if the users email is not confirmed
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ModelState.AddModelError("ForgotPasswordModel.Email", "Please confirm your email to reset your password.");
            }
            // will occur if something unexpected happens
            else
            {
                ModelState.AddModelError("ForgotPasswordModel.Email", "An unexpected error occurred. Please try again.");
            }
            // returnign to the login page with the model
            return View("Login", model);
        }


        // logs the user out and clears the session and re-directs the user to the home page.
        public async Task<IActionResult> Logout()
        {
            var response = await _httpClient.PostAsync("api/users/logout", null);
            if (response.IsSuccessStatusCode) {
                HttpContext.Session.Clear();  
            }
            return RedirectToAction("Index", "Home");
        }

        // Redirect To Appropriate Page
        public IActionResult Redirection() {
            string page = HttpContext.Session.GetString("LastPage");

            if (page != null) {
                if (page == "Peak" || page == "Forum") {
                    int? id = HttpContext.Session.GetInt32("ID");
                    if (id != null) {
                        if (page == "Peak") return RedirectToAction("Index", page, new { ID = id.Value });
                        else return RedirectToAction("Index", page, new { boardID = id.Value });
                    }
                } 

                return RedirectToAction("Index", page);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}