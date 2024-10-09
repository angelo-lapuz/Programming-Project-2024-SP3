using WebAPI.Data;
using WebAPI.Models.Repository;

namespace WebAPI.Models.DataManager;

public class LikeManager : IDataRepository<Like, int>
{
    private readonly PeakHubContext _context;

    public LikeManager(PeakHubContext context)
    {
        _context = context;
    }

    public Like Get(int id)
    {
        return _context.Likes.Find(id);
    }

    public IEnumerable<Like> GetAll()
    {
        return _context.Likes.ToList();
    }

    public int Add(Like like)
    {
        _context.Likes.Add(like);
        _context.SaveChanges();

        return like.LikeID;
    }

    public int Delete(int id)
    {
        _context.Likes.Remove(_context.Likes.Find(id));
        _context.SaveChanges();

        return id;
    }

    public void DeleteLike(int postID, string userID) {
        var like = _context.Likes.FirstOrDefault(l => l.PostID == postID && l.UserId == userID);

        if (like != null) {
            _context.Likes.Remove(like);
            _context.SaveChanges();
        }
    }

    public void LikePost(int postID, string userID) {
        bool hasUserLiked = HasUserLikedPost(postID, userID);

        if (!hasUserLiked) {
            _context.Likes.Add(new Like { PostID = postID, UserId = userID });
            _context.SaveChanges();
        }
    }

    public int Update(int id, Like like)
    {
        _context.Update(like);
        _context.SaveChanges();

        return id;
    }

    public bool HasUserLikedPost(int postID, string userID) {
        return _context.Likes.Any(l => l.PostID == postID && l.UserId == userID);
    }

    public int LikesForPost(int postID) {
        return _context.Likes.Where(l =>  l.PostID == postID).Count();
    }
}
