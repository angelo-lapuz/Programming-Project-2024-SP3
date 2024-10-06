using Microsoft.AspNetCore.Mvc;
using PeakHub.Utilities;
using System.IO;
using System.Linq;

namespace PeakHub.Controllers
{
    public class PeakController : Controller
    {

        private readonly Loader _loader;
        private readonly ILogger<LoginController> _logger;

        public PeakController(Loader loader, ILogger<LoginController> logger)
        {
            _loader = loader;
            _logger = logger;
        }

        public IActionResult Index(int ID)         {

            ViewBag.Peaks = null;
            ViewBag.Peak = null;

            var peak = _loader.GetPeak(ID);
            var peaks = _loader.getAll();
            getImages(peak.Name);

            if(peak == null)
            {
                return NoContent();
            }

            ViewBag.Peak = peak;
            ViewBag.Peaks = Newtonsoft.Json.JsonConvert.SerializeObject(peaks);

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
