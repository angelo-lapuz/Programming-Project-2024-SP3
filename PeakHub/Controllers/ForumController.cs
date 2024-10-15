using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeakHub.Models;
using PeakHub.Utilities;
using PeakHub.ViewModels.Forum;

namespace PeakHub.Controllers {
    public class ForumController : Controller {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ForumController> _logger;
        private readonly string defaultImg = "https://peakhub-user-content.s3.amazonaws.com/default.jpg";
        private readonly Tools _tools;
        private string userID => HttpContext.Session.GetString("UserId");

        public ForumController(IHttpClientFactory httpClient, ILogger<ForumController> logger, Tools tools) {
            _httpClient = httpClient.CreateClient("api");
            _logger = logger;
            _tools = tools;
        }

        public async Task<IActionResult> Index(int boardID) {
            if (await IsInvalidBoardID(boardID)) return RedirectToAction("Index", "Board");

            UserViewModel user = await GetUserDetails();

            Board board = await GetBoard(boardID);

            List<Post> posts = board.Posts;

            ForumViewModel viewModel = new ForumViewModel {
                Board = board,
                User = user,
                Posts = await GetForumPosts(posts, user.userID)
            };

            return View(viewModel);
        }

        public async Task<List<ForumPostViewModel>> GetForumPosts(List<Post> posts, string userID) {
            List<ForumPostViewModel> viewPosts = new();

            foreach (Post post in posts) {

                // this shouldnt be necessary anymore                
                post.User = await GetPostUser(post.UserId);

                if (string.IsNullOrEmpty(post.User.ProfileIMG)) 

                    post.User.ProfileIMG = defaultImg;

                viewPosts.Add(new ForumPostViewModel {
                    UserHasLiked = (await HasUserLikedPost(post.PostID, userID)),
                    LikeCount = await LikesForPost(post.PostID),
                    Post = post
                });
            }

            return viewPosts;
        }

        //--------------------------------------------------------------------------------//
        public async Task<Board> GetBoard(int boardID) =>
            await _httpClient.GetFromJsonAsync<Board>($"api/boards/{boardID}");

        public async Task<User> GetPostUser(string userID) =>
            await _httpClient.GetFromJsonAsync<User>($"api/users/{userID}");

        public async Task<bool> HasUserLikedPost(int postID, string userID) =>
            await _httpClient.GetFromJsonAsync<bool>($"api/likes/{postID}/{userID}");

        public async Task<int> LikesForPost(int postID) =>
            await _httpClient.GetFromJsonAsync<int>($"api/likes/posts/{postID}");

        public async Task<bool> IsInvalidBoardID(int boardID) {
            int boardTotal = await _httpClient.GetFromJsonAsync<int>("api/boards/total");
            return (boardID <= 0 || boardID > boardTotal);
        }


        // -------------------------------------------------------------------------------- //
        public async Task<UserViewModel> GetUserDetails() {
            User user = await _tools.GetUser(userID);

                if (user != null) {
                    return new UserViewModel {
                        userID = user.Id,
                        username = user.UserName,
                        profileImg = (!string.IsNullOrEmpty(user.ProfileIMG)) ? user.ProfileIMG : defaultImg
                    };
                }
            

            return new UserViewModel {
                userID = "0",
                username = "Inconspicuous Andy",
                profileImg = defaultImg
            };
        }
        // -------------------------------------------------------------------------------- //
        [HttpPost]
        public async Task<IActionResult> LikePost(int postID, string userID) {
            var result = await _httpClient.PutAsJsonAsync($"api/likes/add/{postID}/{userID}", new { });

            if (result.IsSuccessStatusCode) {
                int likeCount = await LikesForPost(postID);
                return Json(new { message = "Sucess! A Brilliant Success!", likes = likeCount });
            }
            else return Json(new { message = "Failure! A Devastating Failure!" });
        }

        [HttpDelete]
        public async Task<IActionResult> UnlikePost(int postID, string userID) {
            var result = await _httpClient.DeleteFromJsonAsync<bool>($"api/likes/remove/{postID}/{userID}");

            if (result) {
                int likeCount = await LikesForPost(postID);
                return Json(new { message = "Sucess! A Brilliant Success!", likes = likeCount });
            }
            else return Json(new { message = "Failure! A Devastating Failure!" });
        }
    }
}

