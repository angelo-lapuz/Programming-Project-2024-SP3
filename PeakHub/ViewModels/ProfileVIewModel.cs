using PeakHub.Models;

namespace PeakHub.ViewModels
{
    public class ProfileViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public int TotalCompleted { get; set; }
        public List<Peak> Peaks { get; set; }
        public List<Award> Awards { get; set; }
        public string ProfileImg { get; set; }
    }

}