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


        public async Task<User> GetUser(string userID) {
            _logger.LogInformation("userID = " + userID);

            try {
                var response = await _httpClient.GetAsync($"api/users/{userID}");

                _logger.LogInformation("response : " + response);
                if (!response.IsSuccessStatusCode) {
                    _logger.LogInformation("null : " + response);
                    return null;
                }

                var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
                _logger.LogInformation("User : " + user);

                return user;
            }
            catch (Exception ex) {
                _logger.LogError($"Get User Error: [{ex.Message}]");
                return null;
            }
        }

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
