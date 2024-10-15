using System.Globalization;
using CsvHelper.Configuration;
using PeakHub.Models;
using Newtonsoft.Json;

using Microsoft.AspNetCore.Mvc;

using PeakHub.Controllers;


namespace PeakHub.Utilities
{
    public class Tools
    {
        private readonly HttpClient _httpClient;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<Tools> _logger;

        public Tools(IHttpClientFactory httpClientFactory, IHttpContextAccessor accessor, ILogger<Tools> logger)
        {
            _httpClient = httpClientFactory.CreateClient("api");
            _httpContextAccessor = accessor;
            _logger = logger;
        }


        public async Task<User> GetUser(string userID)
        {
            _logger.LogInformation("userID = " + userID);
            try
            {
                var response = await _httpClient.GetAsync($"api/users/{userID}");

                _logger.LogInformation("response : " + response);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("null : " + response);

                    return null;
                }

                var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());

                _logger.LogInformation("User : " + user);


                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }




    }
}
