using Microsoft.AspNetCore.Mvc;
using PeakHub.Utilities;
using PeakHub.Models;
using PeakHub.ViewModels.Forum;
using Amazon.S3;
using Amazon.S3.Model;
using System.ComponentModel.DataAnnotations;
using PeakHub.ViewModels;

namespace PeakHub.Controllers {
    // -------------------------------------------------------------------------------- //
    public class AddPostRequestData {
        [Required] public string BoardID { get; set; }
        public string Content { get; set; }
        public string Media { get; set; }
        public string MediaType { get; set; }
    }
    // -------------------------------------------------------------------------------- //
    public class AddPostController : Controller {
        private readonly HttpClient _httpClient;
        private readonly IAmazonS3 _s3;
        private readonly ILogger<ForumController> _logger;
        private readonly Tools _tools;

        private string UserID => HttpContext.Session.GetString("UserId");

        public AddPostController(IHttpClientFactory httpClient, IAmazonS3 s3, ILogger<ForumController> logger, Tools tools) {
            _httpClient = httpClient.CreateClient("api");
            _s3 = s3;
            _logger = logger;
            _tools = tools;
        }
        // -------------------------------------------------------------------------------- //
        public async Task<UserViewModel> GetUser() {
            if (string.IsNullOrEmpty(UserID)) return null;
            User user = await _tools.GetUser(UserID);

            return new UserViewModel {
                UserID = user.Id,
                Username = user.UserName,
                ProfileImg = user.ProfileIMG ?? "https://peakhub-user-content.s3.amazonaws.com/default.jpg"
            };
        }
        // -------------------------------------------------------------------------------- //
        [HttpPost]
        public async Task<IActionResult> Index(string BoardID) {
            UserViewModel user = null;

            try { user = await GetUser(); }
            catch (Exception ex) { Console.WriteLine($"Error [{ex.Message}]");  }

            if (user == null) return RedirectToAction("Index", "Forum", new { boardID = BoardID });

            return View(new AddPostViewModel {
                BoardID = BoardID,
                User = user
            });
        }
        // -------------------------------------------------------------------------------- //
        [HttpPost]
        public IActionResult GetPresignedURL([FromBody] PresignedURLDataModel data) {
            string boardID = data.ID, fileType = data.FileType;

            Console.WriteLine("SignedURL START");
            Console.WriteLine($"Board = [{boardID}] | File Type = [{fileType}]");

            if (string.IsNullOrEmpty(fileType)) return Json(new { success = false, message = "File Type is Null" });
            if (string.IsNullOrEmpty(UserID)) return Json(new { success = false, message = "UserID is Null" });
            if (string.IsNullOrEmpty(boardID)) return Json(new { success = false, message = "BoardID is Null" });

            try {
                string dt = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
                string fileExtension = _tools.GetFileExtension(fileType);

                string key = $"{boardID}/{UserID}/{dt}";
                if (!string.IsNullOrEmpty(fileExtension)) { key += $".{fileExtension}"; }

                var request = new GetPreSignedUrlRequest {
                    BucketName = "peakhub-post-content",
                    Key = key,
                    Expires = DateTime.UtcNow.AddMinutes(60),
                    Verb = HttpVerb.PUT,
                    ContentType = fileType
                };

                string presignedURL = _s3.GetPreSignedURL(request);
                Console.WriteLine($"Key = [{key}] \nURL = [{presignedURL}]");
                return Json(new { success = true, url = presignedURL, key = key, type = fileType });
            }
            catch (Exception ex) {
                return Json(new { success = false, message = $"Error [{ex.Message}]" });
            }
        }
        // -------------------------------------------------------------------------------- //
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] AddPostRequestData data) {
            if (data == null) {
                _logger.LogInformation("AddPostRequestData is Null! NULL!!");
                return Json(new { success = false, message = "Server Issue! Please refresh page" });
            }

            data.Content = (data.Content == "NULL") ? null : data.Content;
            data.Media = (data.Media == "NULL") ? null : data.Media;
            data.MediaType = (data.MediaType == "NULL") ? null : data.MediaType;

            if (string.IsNullOrEmpty(data.Content) && string.IsNullOrEmpty(data.Media)) {
                _logger.LogInformation($"Content = [{data.Content}] \nMedia = [{data.Media}]");
                return Json(new { success = false, message = "Ensure at least 1 field is populated" });
            }

            try {
                if (!int.TryParse(data.BoardID, out int boardID) || boardID <= 0) {
                    _logger.LogInformation($"BoardID could not convert to INT || ID is 0");
                    return Json(new { success = false, message = "Server Issue! Please refresh page" });
                }

                Post post = new Post {
                    UserId = UserID,
                    BoardID = boardID,
                    Content = data.Content,
                    MediaType = data.MediaType,
                    MediaLink = data.Media,
                    TransactionTimeUtc = DateTime.Now
                };

                var response = await _httpClient.PostAsJsonAsync("api/posts", post);
                if (!response.IsSuccessStatusCode) throw new Exception("Post Failed Creation!");

                return Json(new { success = true, boardID });
            }
            catch (Exception ex) {
                _logger.LogInformation($"Post Create Error: [{ex.Message}]");
                return Json(new { success = false, message = "Oh No! Something went wrong" });
            }
        }
    }
}
