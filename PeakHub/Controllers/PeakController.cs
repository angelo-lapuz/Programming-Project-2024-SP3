using Microsoft.AspNetCore.Mvc;
using PeakHub.Utilities;
using PeakHub.Models;


namespace PeakHub.Controllers {

    public class PeakController : Controller {

        private readonly Tools _tools;
        private readonly ILogger<PeakController> _logger;
        private readonly HttpClient _httpClient;
        private readonly Lambda_Calls _lambda;
        private string userID => HttpContext.Session.GetString("UserId");

        public PeakController(Lambda_Calls lambda, Tools tools, ILogger<PeakController> logger, IHttpClientFactory httpClientFactory) {
            _tools = tools;
            _logger = logger;
            _lambda = lambda;
            _httpClient = httpClientFactory.CreateClient("api");
        }

        public async Task<IActionResult> Index(int ID)
        {
            // ensuring all viewbag items are null
        public async Task<IActionResult> Index(int ID) {
            HttpContext.Session.SetString("LastPage", "Peak");
            HttpContext.Session.SetInt32("ID", ID);

            ViewBag.Peaks = null;
            ViewBag.Peak = null;
            ViewBag.Routes = null;
            ViewBag.user = userID;

            // get used (if logged in) and load the user routes - set user LoggedIn to true - used in front
            User user = await _tools.GetUser(userID);
            if (user != null)
            {
                ViewBag.Routes = user.Routes;
                ViewBag.userL = true;
            }

            // get peak for this page:
            var getPeakResponse = await _httpClient.GetAsync($"api/Peaks/{ID}");

            // get peak data, serialize and add to viewbag
            if (getPeakResponse.IsSuccessStatusCode)
            {
                var peakData = await getPeakResponse.Content.ReadAsStringAsync();
                var peak = Newtonsoft.Json.JsonConvert.DeserializeObject<Peak>(peakData);
                ViewBag.Peak = peak;


                // get all the Peaks to be displayed on map if user zooms out serialize and add to viewbag
                var getAllPeaksResponse = await _httpClient.GetAsync("api/Peak");
                if (getAllPeaksResponse.IsSuccessStatusCode) {
                    var allpeaks = await getAllPeaksResponse.Content.ReadAsStringAsync();
                    var peaks = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Peak>>(allpeaks);

                    ViewBag.Peaks = Newtonsoft.Json.JsonConvert.SerializeObject(peaks);
                }
                // display view or return to home if peak not found -- should never happen anyway
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // gets the images for thisi specific peak, called from the front due to aws lambda potential wait time.
        [HttpPost("GetPeakImages")]
        public async Task<IActionResult> GetPeakImages([FromBody] peakNameDTO peakName) {
            string mountName = peakName.peakName;
            var pictures = await _lambda.GetPeakPics(mountName);
            // if no images found return not found - return the pictures.
            if (pictures == null)
            {
                return NotFound("No images found");
            }
            return Ok(pictures);
        }

        // peakNameDTO used to call the getPeakImages function - called from the front end js
        public class peakNameDTO
        {
          public string peakName { get; set; }
        }

    }
    }

