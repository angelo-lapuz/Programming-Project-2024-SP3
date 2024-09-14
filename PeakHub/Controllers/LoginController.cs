using Microsoft.AspNetCore.Mvc;

namespace PeakHub.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
