using PeakHub.Models;

namespace PeakHub.ViewModels.Forum
{
    public class ForumViewModel
    {
        public UserViewModel User { get; set; }
        public Board Board { get; set; }
        public ForumViewModel()
        {
            Posts = new List<ForumPostViewModel>();
        }
        public string NewPostContent { get; set; }

        public List<ForumPostViewModel> Posts { get; set; }
        public IFormFile NewPostMediaFile { get; set; }
    }
}
