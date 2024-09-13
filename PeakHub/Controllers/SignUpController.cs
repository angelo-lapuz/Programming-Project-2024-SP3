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
        public IActionResult Create(SignUpViewModel viewModel)
        {
            if (ModelState.IsValid)
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

            return View(viewModel);
        }

        public bool VerifyUserName(string userName)
        {
            var response = await Client.GetAsync("api/movies");
            //var response = await MovieApi.InitializeClient().GetAsync("api/movies");

            if (!response.IsSuccessStatusCode)
                throw new Exception();

            // Storing the response details received from web api.
            var result = await response.Content.ReadAsStringAsync();

            // Deserializing the response received from web api and storing into a list.
            var movies = JsonConvert.DeserializeObject<List<MovieDto>>(result);
        }
    }
}
