namespace PeakHub.ViewModels.Forum {
    public class AddPostViewModel {
        public UserViewModel User { get; set; }
        public string BoardID { get; set; }
        public string postError { get; set; }
        public string Content { get; set; }     
        public string Media { get; set; }
        public string MediaType { get; set; }
    }
}
