using Microsoft.AspNetCore.Mvc;

namespace PeakHub.Controllers
{
    public class PeakController : Controller
    {
   
        public IActionResult Index(int ID)         {


            return View();
        }
    }
}
