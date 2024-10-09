using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography.Pkcs;
using WebApi.Controllers;
using WebAPI.Data;
using WebAPI.Models;
using WebAPI.Models.DataManager;
using WebAPI.Utilities;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileController : ControllerBase
    {
        private readonly PeakHubContext _repo;
        private readonly ILogger<UsersController> _logger;
        private readonly UserManager<User> _userManager;

        public MobileController(PeakHubContext repo, ILogger<UsersController> logger, UserManager<User> userManager)
        {
            _repo = repo;
            _logger = logger;
            _userManager = userManager;
        }



        // checkIn method takes in scanned Data from a QR code, and will process here
        // will check that details are coorrect and will update the user accordingly 
        // will check for potential awards to be earned by the user
        [HttpPost]
        public IActionResult CheckIn([FromBody] ScannedData scannedData)
        {

            /// failedd to recieve data
            if (scannedData == null)
            {
                return BadRequest("Invalid data");
            }

            var peak = _repo.Peaks.FirstOrDefault(p => p.PeakID == scannedData.Id && p.Name == scannedData.peak);

            // should never happen anyway
            if (peak == null)
            {
                return NotFound(new { Message = "Invalid peak" });
            }

            var user = _repo.Users.FirstOrDefault(u => u.Id == scannedData.UserID);

            // should never happen anyway
            if (user == null)
            {
                return NotFound(new { Message = "Invalid user" });
            }

            var completedPeak = _repo.Peaks.Where(p => p.PeakID == scannedData.Id).Any(p => p.Users.Any(u => u.Id == scannedData.UserID));

            // peak already completed by user
            if (completedPeak)
            {
                return Conflict(new { Message = "Peak already completed by user" });
            }

            user.Peaks.Add(peak);
            _repo.SaveChanges();

            var totalPeaks = user.Peaks.Count;
            List<int> awardMilestone = new List<int> { 0, 1, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 158 };

            foreach (int milestone in awardMilestone)
            {
                if (totalPeaks == milestone)
                {
                    var userAward = _repo.Awards.FirstOrDefault(a => a.AwardID == milestone);
                    if (userAward == null)
                    {
                        return BadRequest(new { Message = "Award not found" });
                    }
                    if (!user.Awards.Any(a => a.AwardID == milestone))
                    {
                        user.Awards.Add(userAward);
                        _repo.SaveChanges();
                    }
                }
            }
            _repo.SaveChanges();
            return Ok(new { Message = "Peak checked in successfully" });
        }


        public class ScannedData
        {
            public int Id { get; set; }
            public string peak { get; set; }

            public string UserID { get; set; }
        }
    }
}
