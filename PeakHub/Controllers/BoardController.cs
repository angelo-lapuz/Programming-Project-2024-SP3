using Microsoft.AspNetCore.Mvc;
using PeakHub.Models;
using PeakHub.ViewModels.Forum;
using PeakHub.Utilities;

namespace PeakHub.Controllers {
    public class BoardController : Controller {
        private readonly HttpClient _httpClient;
        private readonly Lambda_Calls _lambda;
        public BoardController(IHttpClientFactory httpClient, Lambda_Calls lambda) {
            _httpClient = httpClient.CreateClient("api");
            _lambda = lambda;
        }

        public async Task<IActionResult> Index() {
            List<string> imgsForOssa = await _lambda.GetPeakPics("Mount_Ossa");


            if (imgsForOssa != null) {
                foreach (string img in imgsForOssa) {
                    Console.WriteLine($"Ossa Img Link: [{img}]");
                }
            }
            else  Console.WriteLine("Image Link FAIL!"); 


            return View(new BoardViewModel { 
                Boards = await _httpClient.GetFromJsonAsync<IEnumerable<Board>>("api/boards")
            });
        }
    }
}
