using Microsoft.AspNetCore.Mvc;
using PeakHub.Utilities;
using PeakHub.ViewModels;

namespace PeakHub.Controllers {
    public class BoardController : Controller {
        public IActionResult Board() {
            ForumContent loader = new();

            BoardViewModel viewModel = new BoardViewModel {
                Boards = loader.GetBoards()
            };

            return View(viewModel);
        }
    }
}
