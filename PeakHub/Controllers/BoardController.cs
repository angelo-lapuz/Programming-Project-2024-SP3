using Microsoft.AspNetCore.Mvc;
using PeakHub.Models;
using PeakHub.ViewModels.Forum;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace PeakHub.Controllers {
    public class BoardController : Controller {
        private readonly HttpClient _httpClient;
        public BoardController(IHttpClientFactory httpClient) =>
            _httpClient = httpClient.CreateClient("api");
        public async Task<IActionResult> Index() {
            return View(new BoardViewModel { 
                Boards = await _httpClient.GetFromJsonAsync<IEnumerable<Board>>("api/boards")
            });
        }
    }
}
