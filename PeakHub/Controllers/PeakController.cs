using Microsoft.AspNetCore.Mvc;
using PeakHub.Utilities;

namespace PeakHub.Controllers
{
    public class PeakController : Controller
    {

        private readonly Loader _loader;

        public PeakController(Loader loader)
        {
            _loader = loader;
        }

        public IActionResult Index(int ID)         {

            ViewBag.Peaks = null;
            ViewBag.Peak = null;

            var peak = _loader.GetPeak(ID);
            var allpeaks = _loader.getAll();

            if(peak == null)
            {
                return NoContent();
            }

            ViewBag.Peak = Newtonsoft.Json.JsonConvert.SerializeObject(peak);
            ViewBag.Peaks = Newtonsoft.Json.JsonConvert.SerializeObject(allpeaks);

            return View();
        }
    }
}
