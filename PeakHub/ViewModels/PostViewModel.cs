using PeakHub.Models;

namespace PeakHub.ViewModels {
    public class PostViewModel {
        public string Username { get; set; }    // Post User's Username
        public string ProfileImg { get; set; }  // Post User's Profile Image [OR Default Image]
        public bool UserHasLikes { get; set; }  // Bool to Determine whether the User has likes the Post
        public Post Post { get; set; }          // The Post to Display
    }
}
