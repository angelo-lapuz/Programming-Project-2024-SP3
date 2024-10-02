using Microsoft.AspNetCore.Mvc;

using PeakHub.Utilities;
namespace PeakHub.Controllers
{
    public class PlannerController : Controller
    {
        private readonly Loader _loader;

        public PlannerController(Loader loader)
        {
            _loader = loader;
        }


        public IActionResult Index()
        {
           var peaks =  _loader.getAll();

            ViewBag.Peaks = Newtonsoft.Json.JsonConvert.SerializeObject(peaks);

            return View();
        }
    }
}
