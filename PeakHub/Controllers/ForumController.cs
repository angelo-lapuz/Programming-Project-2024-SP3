using Microsoft.AspNetCore.Mvc;
using PeakHub.Utilities;
using PeakHub.ViewModels;

namespace PeakHub.Controllers {
    public class ForumController : Controller {
        public IActionResult Forum() {
            ForumContent loader = new();

            ForumViewModel viewModel = new ForumViewModel {
                Username = "gr8Trekkist",
                ProfileImg = "/img/forum_sample.jpg",
                Posts = loader.PostGenerator(10),
                PostsLikedByUser = new()
            };

            return View(viewModel);
        }
    }
}
