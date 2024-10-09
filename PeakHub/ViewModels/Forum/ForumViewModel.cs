using PeakHub.Models;

namespace PeakHub.ViewModels.Forum {
    public class ForumViewModel {
        public UserViewModel User { get; set; } // Necessary User Details
        public Board Board { get; set; } // The Board the User is currently In
        public List<ForumPostViewModel> Posts { get; set; } // All Posts for this Board
    }
}
