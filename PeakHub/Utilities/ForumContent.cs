using CsvHelper.Configuration;
using PeakHub.Models;
using PeakHub.ViewModels;
using System.Globalization;
using System.Security.Policy;

namespace PeakHub.Utilities {
    public class ForumContent {
        // Temp User Storage
        public struct UserTemp {
            public string username { get; set; }
            public string profileImg { get; set; }
        }

        // Board Page - Get Boards
        public List<BoardItemViewModel> GetBoards() {
            List<BoardItemViewModel> boards = new();

            using var reader = new StreamReader("wwwroot/dummyData/boards.csv");
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

        public BoardItemViewModel GetBoard(int id) {
            using var reader = new StreamReader("wwwroot/dummyData/boards.csv");
            using var csv = new CsvHelper.CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) {
                HasHeaderRecord = true
            });

            csv.Read();
            csv.ReadHeader();

            while (csv.Read()) {
                int ID = csv.GetField<int>("Rank");

                if (ID == id) {
                    return new BoardItemViewModel {
                        ID = csv.GetField<int>("Rank"),
                        Name = csv.GetField<string>("Peak"),
                        Section = csv.GetField<string>("Section"),
                        Image = csv.GetField<string>("ImageLink")
                    };
                }
            }

            return null;
        }

        // Forum Page

        public List<UserTemp> GetUsers() {
            List<UserTemp> users = new();

            using var reader = new StreamReader("wwwroot/dummyData/dummy_users.csv");
            using var csv = new CsvHelper.CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) {
                HasHeaderRecord = true
            });

            csv.Read();
            csv.ReadHeader();

            while (csv.Read()) {
                users.Add(new UserTemp { 
                    username = csv.GetField<string>("Username"), 
                    profileImg =  csv.GetField<string>("Profile Image") 
                });
            }

            return users;
        }

        public List<Like> GetLikes() {
            List<Like> likes = new();

            using var reader = new StreamReader("wwwroot/dummyData/dummy_likes.csv");
            using var csv = new CsvHelper.CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) {
                HasHeaderRecord = true
            });

            csv.Read();
            csv.ReadHeader();

            while (csv.Read()) {
                likes.Add(new Like { 
                    LikeID = csv.GetField<int>("LikeID"),
                    PostID = csv.GetField<int>("PostID"),
                    UserID = csv.GetField<string>("UserID")
                });
            }

            return likes;
        }

        public List<Like> SiftLikes(int postID, List<Like> likes) {
            return likes.Where(like => like.PostID == postID).ToList();
        }

        public List<Post> GetPosts(int boardID) {
            List<Like> likes = GetLikes();
            List<Post> posts = new();

            using var reader = new StreamReader("wwwroot/dummyData/dummy_posts.csv");
            using var csv = new CsvHelper.CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) {
                HasHeaderRecord = true
            });

            csv.Read();
            csv.ReadHeader();

            while (csv.Read()) { 
                int PostID = csv.GetField<int>("PostID");
                int BoardID = csv.GetField<int>("BoardID");

                if (BoardID == boardID) {
                    posts.Add(new Post {
                        PostID = PostID,
                        UserId = csv.GetField<string>("UserID"),
                        TransactionTimeUtc = csv.GetField<DateTime>("TransactionTimeUTC"),
                        MediaLink = csv.GetField<string>("Media Link"),
                        MediaType = csv.GetField<string>("Media Type"),
                        BoardID = BoardID,
                        Content = csv.GetField<string>("Content"),
                        Likes = SiftLikes(PostID, likes)
                    });
                }
            }

            return posts;
        }

        public List<PostViewModel> PostGenerator(int boardID) {
            List<Post> posts = GetPosts(boardID);
            List<UserTemp> users = GetUsers();
            List<PostViewModel> postViewModels = new();

            var userDictionary = users.ToDictionary(u => u.username, u => u);

            foreach (Post post in posts) {
                UserTemp user = userDictionary.TryGetValue(post.UserId, out var foundUser) ? foundUser
                    : new UserTemp { username = "gr8Trekkist", profileImg = "/img/forum_sample.jpg" };

                postViewModels.Add(new PostViewModel {
                    Username = user.username,
                    ProfileImg = user.profileImg,
                    Post = post,
                    LikeIcon = "/img/Like_Icon.png"
                });
            }

            return postViewModels;
        }
    }
}
