using Newtonsoft.Json;
using PeakHub.Models;
using SimpleHashing.Net;
using System.Net.Mail;

namespace PeakHub.Utilities
{
    public class WebAPIUtilities
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");

        public WebAPIUtilities(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

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

        public async Task<bool> VerifyPassword(string userName, string password)
        {
            ISimpleHash simpleHash = new SimpleHash();
            List<User> users = await GetUsers();
            bool isPasswordValid = false;
            foreach (var user in users)
            {
                if (user.UserName == userName)
                {
                    isPasswordValid = simpleHash.Verify(password, user.Password);
                }
            }
            return isPasswordValid;
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

        // -------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------- //

        // Adam's Additions / Upgrades [For LogIn + SignUp]

        // Get User via Username
        public async Task<User> GetUser(string userName) {
            var response = await Client.GetAsync($"api/users/username/{userName}");
            if (response.IsSuccessStatusCode) {
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<User>(result);
            }

            return null;
        }

        // Check if Username Exists
        public async Task<bool> VerifyUsername(string userName) {
            var response = await Client.GetAsync($"api/users/username/{userName}");

            if (response.IsSuccessStatusCode) {
                var result = await response.Content.ReadAsStringAsync();
                User user = JsonConvert.DeserializeObject<User>(result);

                return user != null;
            }

            return false;
        }

        // Check if Email Exists
        public async Task<bool> VerifyEmail(string email) {
            var response = await Client.GetAsync($"api/users/email/{email}");

            if (response.IsSuccessStatusCode) {
                var result = await response.Content.ReadAsStringAsync();
                User user = JsonConvert.DeserializeObject<User>(result);

                return user != null;
            }

            return false;
        }

        // Check Password Validity [Not Necessary]
        public async Task<bool> VerifyPass(string userName, string pass) {
            User user = await GetUser(userName);

            if (user != null) {
                SimpleHash simpleHash = new();
                return simpleHash.Verify(pass, user.Password);
            }

            return false;
        }
    }
}
