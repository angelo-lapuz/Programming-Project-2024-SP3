using Microsoft.AspNetCore.Mvc;
using PeakHub.Utilities;
using PeakHub.ViewModels;

namespace PeakHub.Controllers {
    public class BoardController : Controller {
        public IActionResult Board() {
            BoardLoader loader = new();

            BoardViewModel viewModel = new BoardViewModel {
                Boards = loader.getBoards()
            };

            return View(viewModel);
        }
    }
}
