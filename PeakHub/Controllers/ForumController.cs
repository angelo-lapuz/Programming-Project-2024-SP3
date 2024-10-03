using Microsoft.AspNetCore.Mvc;
using PeakHub.ViewModels;

namespace PeakHub.Controllers {
    public class ForumController : Controller {
        public IActionResult Forum() {
            string username = "gr8Trekkist";
            string profileImg = Url.Content("~/img/forum_sample.jpg");
            List<PostViewModel> posts = PostGenerator(10);
            List<int> postsLikedByUser = new();

            foreach (PostViewModel post in posts) {
                Console.WriteLine($"Username: {post.Username}       | Img Link: {post.ProfileImg}");
                Console.WriteLine($"PostID = {post.Post.PostID}     | UserID = {post.Post.UserId}");
            }

            ForumViewModel viewModel = new ForumViewModel {
                Username = username,
                ProfileImg = profileImg,
                Posts = posts,
                PostsLikedByUser = postsLikedByUser
            };

            return View(viewModel);
        }

        private List<PostViewModel> PostGenerator(int amount) {
            List<PostViewModel> postsGenerated = new();
            string postContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";

            string id_1 = "1234",            id_2 = "5678",         id_3 = "9012",
                   user_1 = "theTrekMaster", user_2 = "theTrekman", user_3 = "trekExtrodinaire";

            string img_1 = Url.Content("~/img/sample_1.jpeg");
            string img_2 = Url.Content("~/img/sample_2.jpeg");
            string img_3 = Url.Content("~/img/sample_3.jpg");

            for (int i = 0; i < amount; i++) {
                string id = id_1, user = user_1, img = img_1;

                if (i % 3 == 0) {
                    id = id_2;
                    user = user_2;
                    img = img_2;
                } else if (i % 4 == 0) {
                    id = id_3;
                    user = user_3;
                    img = img_3;
                }

                postsGenerated.Add(new PostViewModel {
                    Username = user,
                    ProfileImg = img,
                    Post = new Models.Post {
                        PostID = i,
                        UserId = id,
                        Content = postContent,
                        TransactionTimeUtc = DateTime.Now,
                        Likes = new(),
                        MediaType = "",
                        MediaLink = "",
                        BoardID = i
                    },
                    LikeIcon = Url.Content("~/img/Like_Icon.png")
                });
            }

            return postsGenerated;
        }
    }
}
