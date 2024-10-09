using PeakHub.Models;

namespace PeakHub.ViewModels.Forum {
    public class BoardViewModel {
        // All Boards to Display
        public IEnumerable<Board> Boards { get; set; } 
    }
}
