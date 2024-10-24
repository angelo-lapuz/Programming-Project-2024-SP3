using System.Globalization;
using CsvHelper.Configuration;
using PeakHub.Models;
using Newtonsoft.Json;

using Microsoft.AspNetCore.Mvc;

using PeakHub.Controllers;


namespace PeakHub.Utilities {
    public class Tools {
        private readonly HttpClient _httpClient;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<Tools> _logger;

        public Tools(IHttpClientFactory httpClientFactory, IHttpContextAccessor accessor, ILogger<Tools> logger) {
            _httpClient = httpClientFactory.CreateClient("api");
            _httpContextAccessor = accessor;
            _logger = logger;
        }


        // GetUser method - used to reduce number of times all this needs to be written: 
        public async Task<User> GetUser(string userID) {

            try {
                // try to get the user from the backend
                var response = await _httpClient.GetAsync($"api/users/{userID}");

                if (!response.IsSuccessStatusCode) {
                    return null;
                }

                // get and de-serialize the user into object and return it to calling method
                var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
                return user;
            }
            // if the user is not found, return null
            catch (Exception ex) {
                return null;
            }
        }

        // used to get various file extensions that can be uploaded to the site
        public string GetFileExtension(string fileType) {
            switch (fileType.ToLower()) {
                // Images
                case "image/jpeg":
                case "image/pjpeg":
                    return ".jpg";
                case "image/png":
                    return ".png";
                case "image/gif":
                    return ".gif";
                case "image/bmp":
                    return ".bmp";
                case "image/webp":
                    return ".webp";
                case "image/tiff":
                    return ".tiff";
                case "image/svg+xml":
                    return ".svg";

                // Videos
                case "video/mp4":
                    return ".mp4";
                case "video/webm":
                    return ".webm";
                case "video/ogg":
                    return ".ogv";
                case "video/x-msvideo":
                    return ".avi";
                case "video/x-ms-wmv":
                    return ".wmv";
                case "video/mpeg":
                    return ".mpeg";

                // GPX (GPS Exchange Format)
                case "application/gpx+xml":
                    return ".gpx";

                // Default
                default:
                    return string.Empty;
            }
        }
    }
}
