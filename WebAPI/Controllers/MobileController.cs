using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models;


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
        [HttpPost("CheckIn")]
        public async Task<IActionResult> CheckIn([FromBody] ScannedData scannedData)
        {
            // check if the scanned data is not null
            if (scannedData == null)
            {
                return BadRequest(new { Message = "Invalid data" });
            }

            // attmempt to find the peak in the database else return error
            var peak = await _repo.Peaks.FirstOrDefaultAsync(p => p.PeakID == scannedData.Id && p.Name == scannedData.peak);
            if (peak == null)
            {
                return NotFound(new { Message = "Invalid peak" });
            }

            // attempt to find the user in the database else return error
            var user = await _repo.Users.FirstOrDefaultAsync(u => u.Id == scannedData.UserID);
            if (user == null)
            {
                return NotFound(new { Message = "Invalid user" });
            }

            // attmept to find if the user has already completed the peak else return error
            var completedPeak = await _repo.UserPeaks.AnyAsync(up => up.UserID == user.Id && up.PeakID == peak.PeakID);
            if (completedPeak)
            {
                return Conflict(new { Message = "Peak already completed by user" });
            }

            // adding the new user peak to the user
            user.UserPeaks.Add(new UserPeak { UserID = user.Id, PeakID = peak.PeakID });

            // required number of peaks for the user to get an award
            List<int> awardMilestone = new List<int> { 0, 1, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 158 };

            // check if the user has reached the required number of peaks for an award
            foreach (int milestone in awardMilestone)
            {
                if (user.UserPeaks.Count == milestone)
                {
                    // check if the user ahs the award already
                    var userAward = await _repo.UserAwards.AnyAsync(ua => ua.UserID == user.Id && ua.AwardID == milestone);
                    if (!userAward)
                    {
                        // add the award to the user
                        _repo.UserAwards.Add(new UserAward { UserID = user.Id, AwardID = milestone });
                    }
                }
            }

            // save changes and return success message
            await _repo.SaveChangesAsync();
            return Ok(new { Message = "Peak checked in successfully" });
        }


        // scanned data DTO - data contained within the QR code
        public class ScannedData
        {
            public int Id { get; set; }
            public string peak { get; set; }
            public string UserID { get; set; }
        }
    }
}
