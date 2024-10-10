using PeakHub.Models;

namespace PeakHub.ViewModels.Forum {
    public class ForumViewModel {
        public ForumViewModel() => Posts = new List<ForumPostViewModel>();

        public UserViewModel User { get; set; }
        public Board Board { get; set; }
        public List<ForumPostViewModel> Posts { get; set; }

        public string postError { get; set; }
    }
}
