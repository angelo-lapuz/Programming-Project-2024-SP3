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

                // Audio
                case "audio/mpeg":
                    return ".mp3";
                case "audio/ogg":
                    return ".ogg";
                case "audio/wav":
                    return ".wav";
                case "audio/x-wav":
                    return ".wav";
                case "audio/webm":
                    return ".webm";
                case "audio/aac":
                    return ".aac";
                case "audio/flac":
                    return ".flac";
                case "audio/mp4":
                    return ".m4a";

                // Documents
                case "application/pdf":
                    return ".pdf";
                case "application/msword":
                    return ".doc";
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    return ".docx";
                case "application/vnd.ms-excel":
                    return ".xls";
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    return ".xlsx";
                case "application/vnd.ms-powerpoint":
                    return ".ppt";
                case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                    return ".pptx";

                // Archives
                case "application/zip":
                    return ".zip";
                case "application/x-tar":
                    return ".tar";
                case "application/gzip":
                    return ".gz";
                case "application/x-7z-compressed":
                    return ".7z";
                case "application/x-rar-compressed":
                    return ".rar";

                // Text files
                case "text/plain":
                    return ".txt";
                case "text/html":
                    return ".html";
                case "text/css":
                    return ".css";
                case "text/javascript":
                    return ".js";
                case "application/json":
                    return ".json";
                case "application/xml":
                    return ".xml";

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
