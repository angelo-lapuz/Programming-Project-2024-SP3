using Microsoft.AspNetCore.Mvc;
using PeakHub.Utilities;
using PeakHub.ViewModels;

namespace PeakHub.Controllers {
    public class ForumController : Controller {
        public IActionResult Forum(int boardID) {
            if (boardID > 0 && boardID <= 158) {
                ForumContent loader = new();

                ForumViewModel viewModel = new ForumViewModel {
                    Username = "AlexYo629",
                    ProfileImg = "https://picsum.photos/seed/21/200",
                    Board = loader.GetBoard(boardID),
                    Posts = loader.PostGenerator(boardID, "AlexYo629"),
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
