using PeakHub.Models;

namespace PeakHub.ViewModels {
    public class PostViewModel {
        public string Username { get; set; }    // Post User's Username
        public string ProfileImg { get; set; }  // Post User's Profile Image [OR Default Image]
        public Post Post { get; set; }          // The Post to Display
        public string LikeIcon { get; set; }    // Icon for the Likes
    }
}
