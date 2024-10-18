using System.ComponentModel.DataAnnotations;

namespace WebAPI.ViewModels {
    public class PostViewModel {
        public int PostID { get; set; }
        public string Content { get; set; }
        public string Media { get; set; }
        public int LikeCount { get; set; }
        public bool HasUserLiked { get; set; }
        public UserViewModel User { get; set; }
        [DataType(DataType.DateTime)] public DateTime TransactionTimeUTC { get; set; }
    }

    public class UserViewModel {
        public string UserID { get; set; }
        public string Username { get; set; }
        public string ProfileImg { get; set; }
    }
}
