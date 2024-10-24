using Microsoft.AspNetCore.Mvc;
using PeakHub.Models;
using PeakHub.ViewModels.Forum;

namespace PeakHub.Controllers {
    public class BoardController : Controller {
        // -------------------------------------------------------------------------------- //
        private readonly HttpClient _httpClient;
        public BoardController(IHttpClientFactory httpClient) =>
            _httpClient = httpClient.CreateClient("api");
        // -------------------------------------------------------------------------------- //
        public IActionResult Index() {
            HttpContext.Session.SetString("LastPage", "Board");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetBoards(int pageIndex) {
            var response = 
                await _httpClient.GetFromJsonAsync<IEnumerable<BoardViewModel>>($"api/boards/load/{pageIndex}");

            return Json(response);
        }
        // -------------------------------------------------------------------------------- //
    }
}
