using Microsoft.AspNetCore.Mvc;
using PeakHub.Utilities;
using PeakHub.ViewModels;

namespace PeakHub.Controllers {
    public class ForumController : Controller {
        public IActionResult Forum(int boardID) {
            if (boardID > 0 && boardID <= 158) {
                ForumContent loader = new();

                ForumViewModel viewModel = new ForumViewModel {
                    Username = "gr8Trekkist",
                    ProfileImg = "/img/forum_sample.jpg",
                    Posts = loader.PostGenerator(boardID),
                    PostsLikedByUser = new()
                };

                return View(viewModel);
            }
            else {
                return RedirectToAction("Board", "Board");
            }
        }
    }
}
