using Microsoft.AspNetCore.Mvc;
using PeakHub.Utilities;
using PeakHub.Models;
using System.Net.Http;

namespace PeakHub.Controllers
{
    public class PeakController : Controller
    {

        private readonly Tools _loader;
        private readonly ILogger<PeakController> _logger;
        private readonly HttpClient _httpClient;


        public PeakController(Tools loader, ILogger<PeakController> logger, IHttpClientFactory httpClientFactory)
        {
            _loader = loader;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("api");
        }

        public async Task<IActionResult> Index(int ID){

            ViewBag.Peaks = null;
            ViewBag.Peak = null;
            ViewBag.Routes = null;

            User user = _loader.GetUser().GetAwaiter().GetResult();

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
                getImages(peak.Name);

            }
            else
            {
                return NoContent();
            }

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

        public void getImages(string peakname)
        {
            peakname = peakname.Replace(" ", "_");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "images", peakname);
            _logger.LogInformation(path);
            _logger.LogInformation(peakname);
            if (Directory.Exists(path))
            {
                var images = Directory.GetFiles(path).Where(x => x.EndsWith(".jpg") || x.EndsWith(".png"))
                    .Select(file => Path.Combine(peakname, Path.GetFileName(file)))
                    .ToList();                
                foreach(var img in images)
                {
                    _logger.LogInformation(img);
                }

                ViewBag.Images = images;

            }
        }

      
    }
}
