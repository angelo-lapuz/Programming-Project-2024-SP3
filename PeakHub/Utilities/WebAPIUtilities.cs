using Newtonsoft.Json;
using PeakHub.Models;
using SimpleHashing.Net;

namespace PeakHub.Utilities
{
    public class WebAPIUtilities
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");

        public WebAPIUtilities(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

       

        public async Task<bool> CheckUserEmailAndName(string userName, string email)
        {
            var response = await Client.GetAsync($"api/Auth/VerifyUserNameAndEmail/{email}{userName}");

            return response.IsSuccessStatusCode ? true : false;
        }

        // Verifies the password for a given user
        public bool VerifyPassword(string password, string hashedPassword)
        {
            ISimpleHash simpleHash = new SimpleHash();
            return simpleHash.Verify(password, hashedPassword);
        }


        // Gets all users using the Get method in the WebAPI project
        // and adds them to a list.
        public async Task<User> GetUser(string userName)
        {
            var response = await Client.GetAsync($"api/users/{userName}");

            if (!response.IsSuccessStatusCode)
                throw new Exception();

            // Storing the response details received from web api.
            var result = await response.Content.ReadAsStringAsync();

            // Deserializing the response received from web api 
            var user = JsonConvert.DeserializeObject<User>(result);

            return user;
        }

        public async Task<User> login(string userName, string password)
        {
            var response = await Client.GetAsync($"api/auth/VerifyLogin/{userName}{password}");

            var result = await response.Content.ReadAsStringAsync();

          
            var user = JsonConvert.DeserializeObject<User>(result);

            return user;
        }
       
    }
}
