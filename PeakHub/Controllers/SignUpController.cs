using Microsoft.AspNetCore.Mvc;
using SimpleHashing.Net;
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
            bool userNameFound = await VerifyUserName(viewModel.UserName);
            bool emailFound = await FindEmail(viewModel.Email);

            if (ModelState.IsValid && !userNameFound && !emailFound)
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

                var response = Client.PostAsync("api/users", content).Result;


                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login", "Login");
                }
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

        public async Task<int> GetUserID(string userName)
        {
            var response = await Client.GetAsync("api/users");


            if (!response.IsSuccessStatusCode)
                throw new Exception();

            // Storing the response details received from web api.
            var result = await response.Content.ReadAsStringAsync();

            // Deserializing the response received from web api and storing into a list.
            var users = JsonConvert.DeserializeObject<List<User>>(result);
            int id = 0;
            foreach (var user in users)
            {
                if (user.UserName == userName)
                {
                    id = user.UserID;
                }
            }
            return id;
        }

    }
}
