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

/*

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

            var completedPeak = _repo.UserPeaks.Any(up => up.UserID == user.Id && up.PeakID == peak.PeakID);

            // peak already completed by user
            if (completedPeak)
            {
                return Conflict(new { Message = "Peak already completed by user" });
            }


            user.UserPeaks.Add(new UserPeak { UserID = user.Id, PeakID = peak.PeakID });
            _repo.SaveChanges();

            var totalPeaks = user.UserPeaks.Count;
            List<int> awardMilestone = new List<int> { 0, 1, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 158 };

            foreach (int milestone in awardMilestone)
            {
                if (totalPeaks == milestone)
                {
                    var userAward = _repo.UserAwards.Any(ua => ua.UserID == user.Id && ua.AwardID == milestone);

                    if (userAward == null)
                    {
                        return BadRequest(new { Message = "Award not found" });
                    }

                    if (!userAward)
                    {
                        _repo.UserAwards.Add(new UserAward { UserID = user.Id, AwardID = milestone });
                        _repo.SaveChanges();
                    }
                }
            }
            _repo.SaveChanges();
            return Ok(new { Message = "Peak checked in successfully" });
        }
*/

        public class ScannedData
        {
            public int Id { get; set; }
            public string peak { get; set; }

            public string UserID { get; set; }
        }
    }
}
