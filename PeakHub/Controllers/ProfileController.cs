using Microsoft.AspNetCore.Mvc;

namespace PeakHub.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
