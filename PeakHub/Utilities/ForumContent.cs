using CsvHelper.Configuration;
using PeakHub.Models;
using PeakHub.ViewModels;
using System.Globalization;
using System.Security.Policy;

namespace PeakHub.Utilities {
    public class ForumContent {
        public List<BoardItemViewModel> GetBoards() {
            List<BoardItemViewModel> boards = new();

            using var reader = new StreamReader("wwwroot/boards.csv");
            using var csv = new CsvHelper.CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) {
                HasHeaderRecord = true
            });

            csv.Read();
            csv.ReadHeader();

            while (csv.Read()) {
                boards.Add(new BoardItemViewModel {
                    ID = csv.GetField<int>("Rank"),
                    Name = csv.GetField<string>("Peak"),
                    Section = csv.GetField<string>("Section"),
                    Image = csv.GetField<string>("ImageLink")
                });
            }

            return boards;
        }

        public List<PostViewModel> PostGenerator(int amount) {
            List<PostViewModel> posts = new();

            // Dummy Post Content
            string postContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";

            // Dummy User Info
            var users = new (string id, string username, string profileImg)[] {
                ("1234", "theTrekMaster", "/img/sample_1.jpeg"),
                ("5678", "theTrekman", "/img/sample_2.jpeg"),
                ("9012", "trekExtrodinaire", "/img/sample_3.jpg")
            };

            for (int i = 0; i < amount; i++) {
                var user = users[i % users.Length];

                posts.Add(new PostViewModel {
                    Username = user.username,
                    ProfileImg = user.profileImg,
                    LikeIcon = "/img/Like_Icon.png",
                    Post = new Post {
                        PostID = i,
                        UserId = user.id,
                        Content = postContent,
                        TransactionTimeUtc = DateTime.Now,
                        Likes = new(),
                        MediaType = "",
                        MediaLink = "",
                        BoardID = i
                    }
                });
            }

            return posts;
        }
    }
}
