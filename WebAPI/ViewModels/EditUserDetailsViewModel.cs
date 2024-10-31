namespace WebAPI.ViewModels
{
    public class EditUserDetailsViewModel
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }

        // a check to see if user is locked in the
        // database
        public bool IsLocked { get; set; }

        // A check to see if admin requested to
        // ban or unban user
        public bool BanUser { get; set; }

        //public int PhoneNumber { get; set; }
    }
}

