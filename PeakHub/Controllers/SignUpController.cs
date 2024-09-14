using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeakHub.ViewModels;
using PeakHub.Models;
using System.Text;

namespace PeakHub.Controllers
{
    public class SignUpController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");

        public SignUpController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Create()
        {

            return View();
        }

        // POST: SignUp/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SignUpViewModel viewModel)
        {
            bool userNameFound = await VerifyUserName(viewModel.UserName);
            bool emailFound = await FindEmail(viewModel.Email);

            if (ModelState.IsValid && !userNameFound && !emailFound)
            {
                var user = new User
                {
                    UserName = viewModel.UserName,
                    Email = viewModel.Email,
                    Password = viewModel.Password,
                };

                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                var response = Client.PostAsync("api/users", content).Result;


                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index", "Home");
            }
            if (userNameFound) 
            {
                ModelState.AddModelError("UserName", "Username exists");
            }
            if (emailFound) 
            {
                ModelState.AddModelError("Email", "Email already in use");
            }

            return View(viewModel);
        }

        // Calls GetUsers() to get all the users from the database
        // and iterates through the list to find matching username
        // returns true if found / false if not.
        public async Task<bool> VerifyUserName(string userName) 
        {
            List<User> users = await GetUsers();
            bool userFound = false;

            foreach (var user in users)
            {
                if (user.UserName == userName)
                {
                    userFound = true;
                }
            }

            return userFound;
        }

        // Calls GetUsers() to get all the users from the database
        // and iterates through the list to find matching email
        // returns true if found / false if not.
        public async Task<bool> FindEmail(string email)
        {
            List<User> users = await GetUsers();
            bool emailFound = false;

            foreach (var user in users)
            {
                if (user.Email == email)
                {
                    emailFound = true;
                }
            }

            return emailFound;
        }

        // Gets all users using the Get method in the WebAPI project
        // and adds them to a list.
        public async Task<List<User>> GetUsers()
        {
            var response = await Client.GetAsync("api/users");


            if (!response.IsSuccessStatusCode)
                throw new Exception();

            // Storing the response details received from web api.
            var result = await response.Content.ReadAsStringAsync();

            // Deserializing the response received from web api and storing into a list.
            var users = JsonConvert.DeserializeObject<List<User>>(result);

            return users;
        }
    }
}
