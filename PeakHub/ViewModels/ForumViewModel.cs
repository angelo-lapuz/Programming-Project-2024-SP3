using PeakHub.Models;

namespace PeakHub.ViewModels {
    public class ForumViewModel {
        public string Username { get; set; }                // Signed In User's Username
        public string ProfileImg { get; set; }              // Signed In User's Profile Image [OR Default Image]
        public BoardItemViewModel Board {  get; set; }      // The Board of the Page
        public List<PostViewModel> Posts { get; set; }      // All Posts to be displayed
        public List<int> PostsLikedByUser { get; set; }     // A list of Post IDs that are liked by the user

    }
}
