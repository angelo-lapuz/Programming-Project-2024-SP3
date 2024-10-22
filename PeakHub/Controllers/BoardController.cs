using Microsoft.AspNetCore.Mvc;
using PeakHub.Models;

namespace PeakHub.Controllers {
    public class BoardController : Controller {
        // -------------------------------------------------------------------------------- //
        private readonly HttpClient _httpClient;
        public BoardController(IHttpClientFactory httpClient) =>
            _httpClient = httpClient.CreateClient("api");
        // -------------------------------------------------------------------------------- //
        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> AllBoards() {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<Board>>("api/boards");
            return Json(response);
        }
        // -------------------------------------------------------------------------------- //
    }
}
