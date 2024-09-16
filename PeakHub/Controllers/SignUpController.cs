using Microsoft.AspNetCore.Mvc;
using SimpleHashing.Net;
using Newtonsoft.Json;
using PeakHub.ViewModels;
using PeakHub.Models;
using System.Text;
using PeakHub.Utilities;

namespace PeakHub.Controllers
{
    public class SignUpController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        private readonly ILogger<HomeController> _logger;


        private HttpClient Client => _clientFactory.CreateClient("api");

        public SignUpController(IHttpClientFactory clientFactory, ILogger<HomeController> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        WebAPIUtilities utilities => new WebAPIUtilities(_clientFactory);

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        // Takes input given by user from form, verifies is username and email
        // aren't in the database, if one or both are found in the database,
        // a error message is added to the ModelState which is then returned back
        // to the Creat view. If there email and usenrame arent found and there are
        // no errors, SimpleHashing is used to hash the password and a user is
        // created.
        // POST: SignUp/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SignUpViewModel viewModel)
        {
            var response = await Client.GetAsync($"api/users/Verify/{viewModel.UserName}/{viewModel.Email}");

            var resultContent = await response.Content.ReadAsStringAsync();

            dynamic result = JsonConvert.DeserializeObject(resultContent);

            bool usernameExists = result.usernameExists != null ? (bool)result.usernameExists : false;
            bool emailExists = result.emailExists != null ? (bool)result.emailExists : false;

            if (usernameExists)
            {
                ModelState.AddModelError("UserName", "Username exists");
            }

            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email already in use");
            }

            if (ModelState.IsValid)
            {
                ISimpleHash simpleHash = new SimpleHash();
                string hashedPassword = simpleHash.Compute(viewModel.Password);

                var user = new User
                {
                    UserName = viewModel.UserName,
                    Email = viewModel.Email,
                    Password = hashedPassword,
                };

                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                response = Client.PostAsync("api/users", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login", "Login");
                }
            }

            return View(viewModel);
        }
    }
}
