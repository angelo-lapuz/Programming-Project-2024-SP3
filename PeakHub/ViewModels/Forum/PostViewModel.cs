using System.ComponentModel.DataAnnotations;

namespace PeakHub.ViewModels.Forum {
    public class PostViewModel {
        public int PostID { get; set; }
        public string Content { get; set; }
        public string Media { get; set; }
        public int LikeCount { get; set; }
        public bool HasUserLiked { get; set; }
        public UserViewModel User { get; set; }
        [DataType(DataType.DateTime)] public DateTime TransactionTimeUTC { get; set; }
    }
}
