using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeakHub.Models;
using PeakHub.ViewModels.Forum;

namespace PeakHub.Controllers
{
    public class ForumController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ForumController> _logger;
        public ForumController(IHttpClientFactory httpClient, ILogger<ForumController> logger)
        {
            _httpClient = httpClient.CreateClient("api");
            _logger = logger;

        }

        public async Task<IActionResult> Index(int boardID)
        {
            if (await IsInvalidBoardID(boardID)) return RedirectToAction("Index", "Board");

            UserViewModel user = await GetUserDetails();
            Board board = await GetBoard(boardID);
            List<Post> posts = board.Posts;

            ForumViewModel viewModel = new ForumViewModel
            {
                Board = board,
                User = user,
                Posts = await GetForumPosts(posts, user.userID)
            };

            return View(viewModel);
        }

        public async Task<List<ForumPostViewModel>> GetForumPosts(List<Post> posts, string userID)
        {
            List<ForumPostViewModel> viewPosts = new();

            foreach (Post post in posts)
            {
                post.User = await GetPostUser(post.UserId);

                viewPosts.Add(new ForumPostViewModel
                {
                    UserHasLiked = (await HasUserLikedPost(post.PostID, userID)),
                    LikeCount = await LikesForPost(post.PostID),
                    Post = post
                });
            }

            return viewPosts;
        }

        public async Task<Board> GetBoard(int boardID)
        {
            return await _httpClient.GetFromJsonAsync<Board>($"api/boards/{boardID}");
        }

        public async Task<bool> IsInvalidBoardID(int boardID)
        {
            int boardTotal = await _httpClient.GetFromJsonAsync<int>("api/boards/total");
            return (boardID <= 0 || boardID > boardTotal);
        }

        public async Task<UserViewModel> GetUserDetails()
        {
            var response = await _httpClient.GetAsync("api/users/GetUser");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                User user = JsonConvert.DeserializeObject<User>(result);

                if (user != null)
                {
                    return new UserViewModel
                    {
                        userID = user.Id,
                        username = user.UserName,
                        profileImg = (user.ProfileIMG != null) ?
                            user.ProfileIMG : "https://images.pexels.com/photos/6757691/pexels-photo-6757691.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1"
                    };
                }
            }

            return new UserViewModel
            {
                userID = "0",
                username = "Inconspicuous Andy",
                profileImg = "https://images.pexels.com/photos/6757691/pexels-photo-6757691.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1"
            };
        }

        public async Task<User> GetPostUser(string userID)
        {
            return await _httpClient.GetFromJsonAsync<User>($"api/users/{userID}");
        }

        public async Task<bool> HasUserLikedPost(int postID, string userID)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/likes/{postID}/{userID}");
        }

        public async Task<int> LikesForPost(int postID)
        {
            return await _httpClient.GetFromJsonAsync<int>($"api/likes/posts/{postID}");
        }

        // -------------------------------------------------------------------------------- //

        [HttpPost]
        public async Task<IActionResult> LikePost(int postID, string userID)
        {
            var result = await _httpClient.PutAsJsonAsync($"api/likes/add/{postID}/{userID}", new { });

            if (result.IsSuccessStatusCode)
            {
                int likeCount = await LikesForPost(postID);
                return Json(new { message = "Sucess! A Brilliant Success!", likes = likeCount });
            }
            else return Json(new { message = "Failure! A Devastating Failure!" });
        }

        [HttpDelete]
        public async Task<IActionResult> UnlikePost(int postID, string userID)
        {
            var result = await _httpClient.DeleteFromJsonAsync<bool>($"api/likes/remove/{postID}/{userID}");

            if (result)
            {
                int likeCount = await LikesForPost(postID);
                return Json(new { message = "Sucess! A Brilliant Success!", likeCount });
            }
            else return Json(new { message = "Failure! A Devastating Failure!" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ForumViewModel viewModel)
        {
            if (viewModel.User == null)
            {
                viewModel.User = await GetUserDetails();
            }

            if (viewModel.Board == null)
            {
                viewModel.Board = new Board();
            }

            if (viewModel.Board.BoardID == 0)
            {
                ModelState.AddModelError("", "Board information is missing.");
                return View("Index", viewModel);
            }

            if (!ModelState.IsValid)
            {
                return View("Index", viewModel);
            }

            using (var formDataContent = new MultipartFormDataContent())
            {
                formDataContent.Add(new StringContent(viewModel.User.userID), "UserID");
                formDataContent.Add(new StringContent(viewModel.Board.BoardID.ToString()), "BoardID");

                if (!string.IsNullOrEmpty(viewModel.NewPostContent))
                {
                    formDataContent.Add(new StringContent(viewModel.NewPostContent), "Content");
                }
                if (viewModel.NewPostMediaFile != null && viewModel.NewPostMediaFile.Length > 0)
                {
                    var fileContent = new StreamContent(viewModel.NewPostMediaFile.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(viewModel.NewPostMediaFile.ContentType);
                    formDataContent.Add(fileContent, "mediaFile", viewModel.NewPostMediaFile.FileName);
                }

                var response = await _httpClient.PostAsync("api/posts/create", formDataContent);

                if (response.IsSuccessStatusCode)
                {
                    var board = await GetBoard(viewModel.Board.BoardID);
                    viewModel.Posts = await GetForumPosts(board.Posts, viewModel.User.userID);

                    return View("Index", viewModel);
                }

                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
            }

            return View("Index", viewModel);
        }


        private async Task<string> SaveMediaFile(IFormFile mediaFile)
        {
            if (mediaFile == null || mediaFile.Length == 0) return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(mediaFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await mediaFile.CopyToAsync(fileStream);
            }

            _logger.LogInformation("Media file saved at: {FilePath}", filePath);

            return "/uploads/" + uniqueFileName;
        }
    }
}

