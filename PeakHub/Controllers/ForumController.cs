using Microsoft.AspNetCore.Mvc;
using PeakHub.Models;
using PeakHub.Utilities;
using PeakHub.ViewModels.Forum;

namespace PeakHub.Controllers {
    public class ForumController : Controller {
        private readonly HttpClient _httpClient;
        private readonly Tools _tools;
        private string UserID => HttpContext.Session.GetString("UserId");

        public ForumController(IHttpClientFactory httpClient, Tools tools) {
            _httpClient = httpClient.CreateClient("api");
            _tools = tools;
        }
        // -------------------------------------------------------------------------------- //
        public async Task<IActionResult> Index(int boardID) {
            if (await IsInvalidBoardID(boardID)) return RedirectToAction("Index", "Board");
            UserViewModel user = await GetUserDetails();
            
            return View(new ForumViewModel {
                User = user,
                PageIndex = 1,
                BoardID = boardID
            });
        }
        public async Task<bool> IsInvalidBoardID(int boardID) {
            int boardTotal = await _httpClient.GetFromJsonAsync<int>("api/boards/total");
            return (boardID <= 0 || boardID > boardTotal);
        }
        // -------------------------------------------------------------------------------- //
        [HttpGet]
        public async Task<IActionResult> GetForumPosts(int boardID, string userID, int pageSize, int pageIndex) {


            var response = await _httpClient
                .GetFromJsonAsync<IEnumerable<PostViewModel>>($"api/posts/fromBoard/{boardID}/{userID}?pageSize={pageSize}&pageIndex={pageIndex}");

            return Json(response);
        }
        // -------------------------------------------------------------------------------- //
        public async Task<UserViewModel> GetUserDetails() {
            if (string.IsNullOrEmpty(UserID)) {
                return new UserViewModel {
                    UserID = "0",
                    Username = "Inconspicuous Andy",
                    ProfileImg = "https://peakhub-user-content.s3.amazonaws.com/default.jpg"
                };
            }

            User user = await _tools.GetUser(UserID);

            return new UserViewModel {
                UserID = user.Id,
                Username = user.UserName,
                ProfileImg = user.ProfileIMG ?? "https://peakhub-user-content.s3.amazonaws.com/default.jpg"
            };
        }
        // -------------------------------------------------------------------------------- //
        public async Task<int> LikesForPost(int postID) =>
            await _httpClient.GetFromJsonAsync<int>($"api/likes/posts/{postID}");

        [HttpPost]
        public async Task<IActionResult> LikePost(int postID) {
            var result = await _httpClient.PutAsJsonAsync($"api/likes/add/{postID}/{UserID}", new { });

            if (result.IsSuccessStatusCode) {
                int likeCount = await LikesForPost(postID);
                return Json(new { message = "Sucess! A Brilliant Success!", likes = likeCount });
            }
            else return Json(new { message = "Failure! A Devastating Failure!" });
        }

        [HttpDelete]
        public async Task<IActionResult> UnlikePost(int postID) {
            var result = await _httpClient.DeleteFromJsonAsync<bool>($"api/likes/remove/{postID}/{UserID}");

            if (result) {
                int likeCount = await LikesForPost(postID);
                return Json(new { message = "Sucess! A Brilliant Success!", likes = likeCount });
            }
            else return Json(new { message = "Failure! A Devastating Failure!" });
        }
        // -------------------------------------------------------------------------------- //
    }
}

