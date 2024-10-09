using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeakHub.Models;
using PeakHub.ViewModels.Forum;

namespace PeakHub.Controllers {
    public class ForumController : Controller {
        private readonly HttpClient _httpClient;
        public ForumController(IHttpClientFactory httpClient) =>
            _httpClient = httpClient.CreateClient("api");

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

            foreach(Post post in posts) {
                post.User = await GetPostUser(post.UserId);

                viewPosts.Add(new ForumPostViewModel { 
                    UserHasLiked = (await HasUserLikedPost(post.PostID, userID)),
                    LikeCount = await LikesForPost(post.PostID),
                    Post = post
                });
            }

            return viewPosts;
        }

        public async Task<Board> GetBoard(int boardID) {
            return await _httpClient.GetFromJsonAsync<Board>($"api/boards/{boardID}");
        }

        public async Task<bool> IsInvalidBoardID(int boardID) {
            int boardTotal = await _httpClient.GetFromJsonAsync<int>("api/boards/total");
            return (boardID <= 0 || boardID > boardTotal);
        }

        public async Task<UserViewModel> GetUserDetails() {
            var response = await _httpClient.GetAsync("api/users/GetUser");

            if (response.IsSuccessStatusCode) {
                var result = await response.Content.ReadAsStringAsync();
                User user = JsonConvert.DeserializeObject<User>(result);

                if (user != null) {
                    return new UserViewModel {
                        userID = user.Id,
                        username = user.UserName,
                        profileImg = (user.ProfileIMG != null) ? 
                            user.ProfileIMG : "https://images.pexels.com/photos/6757691/pexels-photo-6757691.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1"
                    };
                }
            }

            return new UserViewModel {
                userID = "0",
                username = "Inconspicuous Andy",
                profileImg = "https://images.pexels.com/photos/6757691/pexels-photo-6757691.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1"
            };
        }

        public async Task<User> GetPostUser(string userID) {
            return await _httpClient.GetFromJsonAsync<User>($"api/users/{userID}");
        }

        public async Task<bool> HasUserLikedPost(int postID, string userID) {
            return await _httpClient.GetFromJsonAsync<bool>($"api/likes/{postID}/{userID}");
        }

        public async Task<int> LikesForPost(int postID) {
            return await _httpClient.GetFromJsonAsync<int>($"api/likes/posts/{postID}");
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
