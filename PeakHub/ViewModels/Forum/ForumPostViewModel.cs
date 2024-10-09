using PeakHub.Models;

namespace PeakHub.ViewModels.Forum {
    public class ForumPostViewModel {
        public bool UserHasLiked { get; set; } // Bool Indicating if Logged In User Liked the Post
        public int LikeCount { get; set; } // Int Indicating Like Total for Post
        public Post Post { get; set; } // The Post Being Displayed
    }
}
