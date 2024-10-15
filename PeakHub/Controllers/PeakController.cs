using Microsoft.AspNetCore.Mvc;
using PeakHub.Utilities;
using PeakHub.Models;
using System.Net.Http;
using Amazon.S3.Model;

namespace PeakHub.Controllers
{

    public class PeakController : Controller
    {

        private readonly Tools _tools;
        private readonly ILogger<PeakController> _logger;
        private readonly HttpClient _httpClient;
        private readonly Lambda_Calls _lambda;
        private string userID => HttpContext.Session.GetString("UserId");

        public PeakController(Lambda_Calls lambda, Tools tools, ILogger<PeakController> logger, IHttpClientFactory httpClientFactory)
        {
            _tools = tools;
            _logger = logger;
            _lambda = lambda;
            _httpClient = httpClientFactory.CreateClient("api");
        }

        public async Task<IActionResult> Index(int ID)
        {

            ViewBag.Peaks = null;
            ViewBag.Peak = null;
            ViewBag.Routes = null;
            ViewBag.user = userID;


            User user = await _tools.GetUser(userID);

            if (user != null)
            {
                ViewBag.Routes = user.Routes;
                ViewBag.userL = true;
            }


            // get peak for this page:
            var getPeakResponse = await _httpClient.GetAsync($"api/Peaks/{ID}");

            if (getPeakResponse.IsSuccessStatusCode)
            {

                var peakData = await getPeakResponse.Content.ReadAsStringAsync();
                var peak = Newtonsoft.Json.JsonConvert.DeserializeObject<Peak>(peakData);
                ViewBag.Peak = peak;


                // get all the Peaks
                var getAllPeaksResponse = await _httpClient.GetAsync("api/Peak");
                if (getAllPeaksResponse.IsSuccessStatusCode)
                {
                    var allpeaks = await getAllPeaksResponse.Content.ReadAsStringAsync();
                    var peaks = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Peak>>(allpeaks);

                    ViewBag.Peaks = Newtonsoft.Json.JsonConvert.SerializeObject(peaks);
                }

                return View();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("GetPeakImages")]
        public async Task<IActionResult> GetPeakImages([FromBody] peakNameDTO peakName)
        {

            string mountName = peakName.peakName;

            _logger.LogInformation("peakName was" + mountName);
            var pictures = await _lambda.GetPeakPics(mountName);
            if (pictures == null)
            {
                return NotFound("No images found");
            }
            return Ok(pictures);
        }


        public class peakNameDTO
        {
          public string peakName { get; set; }
        }

    }
    }

