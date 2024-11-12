using Microsoft.AspNetCore.Mvc;

namespace PeakHub.Controllers
{
    public class GeneralController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
