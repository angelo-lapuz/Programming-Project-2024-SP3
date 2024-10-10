using Microsoft.AspNetCore.Mvc;
using PeakHub.Utilities;
using PeakHub.Models;
using PeakHub.ViewModels.Forum;

namespace PeakHub.Controllers {
    public class AddPostController : Controller {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ForumController> _logger;
        
        public AddPostController(IHttpClientFactory httpClient, ILogger<ForumController> logger) {
            _httpClient = httpClient.CreateClient("api");
            _logger = logger;
        }
        // -------------------------------------------------------------------------------- //
        public async Task<UserViewModel> GetUser(string id) {
            if (string.IsNullOrEmpty(id)) {
                _logger.LogInformation("UserID IS Null!");
                return null;
            }

            User userObject = await _httpClient.GetFromJsonAsync<User>($"api/users/{id}");

            return new UserViewModel {
                userID = userObject.Id,
                username = userObject.UserName,
                profileImg = (userObject.ProfileIMG != null) ?
                        userObject.ProfileIMG : "https://peakhub-user-content.s3.amazonaws.com/default.jpg"
            };
        }
        // -------------------------------------------------------------------------------- //
        [HttpPost("AddPost/IndexPage")]
        public async Task<IActionResult> Index(string UserID, string BoardID) {
            UserViewModel user = await GetUser(UserID);
            if (user == null) return RedirectToAction("Index", "Forun", new { boardID = BoardID });

            return View(new AddPostViewModel {
                BoardID = BoardID,
                User = user
            });
        }
        // -------------------------------------------------------------------------------- //
        public async Task<string> HandleMedia(string key, IFormFile mediaFile) {
            var allowedMediaTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp", "video/mp4", "video/webm" };

            if (!allowedMediaTypes.Contains(mediaFile.ContentType))
                return "[Error] Media must be either an Image, MP4, or WebM File";

            try {
                var tempFilePath = Path.GetTempFileName();
                using (var stream = new FileStream(tempFilePath, FileMode.Create)) {
                    await mediaFile.CopyToAsync(stream);
                }

                AWS_SecretsManager sm = new();
                AWS_S3 _s3 = new(await sm.GetCreds());

                string fileLink = await _s3.UploadToS3("post", key, tempFilePath, mediaFile.ContentType);
                if (string.IsNullOrEmpty(fileLink)) throw new Exception("File Key Missing");

                if (System.IO.File.Exists(tempFilePath)) System.IO.File.Delete(tempFilePath);
                return fileLink;
            }
            catch (Exception ex) {
                _logger.LogInformation($"Image Upload Error: [{ex.Message}]");
                return "[Error] Something Happened While Uploading Your Media";
            }
        }
        // -------------------------------------------------------------------------------- //
        [HttpPost("AddPost/CreatePost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string userID, string boardID, string postContent, IFormFile mediaFile) {
            Console.WriteLine($"Board {boardID} | User = {userID}");
            string fileLink = null, fileType = (mediaFile != null) ? mediaFile.ContentType : null;

            DateTime dt = DateTime.Now;

            if (string.IsNullOrEmpty(postContent) && (mediaFile == null || mediaFile.Length <= 0)) {
                _logger.LogInformation($"Content = {postContent} \nMedia = {mediaFile.FileName} + {mediaFile.ContentType}");
                ModelState.AddModelError("postError", "Ensure at least 1 field is Populated");
            }

            if (ModelState.IsValid && fileType != null) {
                string key = $"{boardID}/{userID}/{dt.ToString("dd-MM-yyyy_HH-mm-ss")}";
                string result = await HandleMedia(key, mediaFile);

                if (result.Contains("[Error]")) ModelState.AddModelError("postError", "Ensure at least 1 field is Populated");
                else fileLink = result;
            }

            if (ModelState.IsValid) {
                try {
                    Post post = new Post {
                        UserId = userID,
                        BoardID = int.Parse(boardID),
                        Content = postContent,
                        MediaType = fileType,
                        MediaLink = fileLink,
                        TransactionTimeUtc = dt
                    };

                    var response = await _httpClient.PostAsJsonAsync("api/posts", post);
                    if (!response.IsSuccessStatusCode) throw new Exception("Post Failed Creation!");

                    return RedirectToAction("Index", "Forum", new { boardID = boardID });
                }
                catch (Exception ex) {
                    _logger.LogInformation($"Post Create Error: [{ex.Message}]");
                    ModelState.AddModelError("postError", "An Error Occured Creating Your Post");
                }
            }

            UserViewModel user = await GetUser(userID);

            return View(new AddPostViewModel {
                BoardID = boardID,
                User = user
            });
        }
    }
}
