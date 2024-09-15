using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeakHub.Utilities;
using PeakHub.ViewModels;
using PeakHub.Models;
using SimpleHashing.Net;
using System.Text;

namespace PeakHub.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");

        public LoginController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login() 
        {
            return View();
        }

        // Takes input given by user from form, verifies is username and email
        // aren't in the database, if one or both are found in the database,
        // a error message is added to the ModelState which is then returned back
        // to the Creat view. If there email and usenrame arent found and there are
        // no errors, SimpleHashing is used to hash the password and a user is
        // created in the database.
        // POST: SignUp/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            WebAPIUtilities utilities = new WebAPIUtilities(_clientFactory);

            bool userNameFound = await utilities.VerifyUserName(viewModel.UserName);


            if (ModelState.IsValid && userNameFound)
            {
                bool passwordsMatch = await utilities.VerifyPassword(viewModel.UserName, viewModel.Password);

                if (!passwordsMatch) 
                {
                    ModelState.AddModelError("LoginError", "Invalid Login details");
                    return View(viewModel);
                }

                List<User> users = await utilities.GetUsers();

                foreach (var user in users) 
                {
                    HttpContext.Session.SetInt32("UserID", user.UserID);
                    HttpContext.Session.SetString("Username", user.UserName);
                }

                return RedirectToAction("Index", "Home");
            }


            return View(viewModel);
        }
    }
}
