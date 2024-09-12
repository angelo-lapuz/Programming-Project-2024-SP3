using Microsoft.AspNetCore.Mvc;

namespace PeakHub.Controllers
{
    public class SignUpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
