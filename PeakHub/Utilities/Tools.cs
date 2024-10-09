using System.Globalization;
using CsvHelper.Configuration;
using PeakHub.Models;
using Newtonsoft.Json;
using PeakHub.Controllers;


namespace PeakHub.Utilities
{
    public class Tools
    {
        private readonly HttpClient _httpClient;

        public Tools(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("api");
        }


        public async Task<User> GetUser()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/users/GetUser");

                if (!response.IsSuccessStatusCode)
                {

                    return null;
                }

                var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());

                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }




    }
}
