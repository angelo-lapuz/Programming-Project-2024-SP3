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

    // gets a specific Like from the database by ID
    public Like Get(int id)
    {
        return _context.Likes.Find(id);
    }

    // gets all Likes from the database
    public IEnumerable<Like> GetAll()
    {
        return _context.Likes.ToList();
    }

    // adds a new Like to the database
    public int Add(Like like)
    {
        _context.Likes.Add(like);
        _context.SaveChanges();
        return like.LikeID;
    }

    // deletes a specific Like from the database by ID
    public int Delete(int id)
    {
        _context.Likes.Remove(_context.Likes.Find(id));
        _context.SaveChanges();
        return id;
    }

    // deletes multiple Likes from the database based on a list of IDs
    public void DeleteAll(List<int> ids)
    {
        foreach (var id in ids)
        {
            _context.Likes.Remove(_context.Likes.Find(id));
        }
        _context.SaveChanges();
    }

    //removes a specific unspecified like from the database based off the post and userID
    public void DeleteLike(int postID, string userID) {
        var like = _context.Likes.FirstOrDefault(l => l.PostID == postID && l.UserId == userID);

        if (like != null) {
            _context.Likes.Remove(like);
            _context.SaveChanges();
        }
    }


    //removes all likes from the database based off the postID
    public void LikePost(int postID, string userID) {
        bool hasUserLiked = HasUserLikedPost(postID, userID);

        if (!hasUserLiked) {
            _context.Likes.Add(new Like { PostID = postID, UserId = userID });
            _context.SaveChanges();
        }
    }

    // updates an existing Like in the database
    public int Update(int id, Like like)
    {
        _context.Update(like);
        _context.SaveChanges();

        return id;
    }

    //checks if a user has liked a specific post
    public bool HasUserLikedPost(int postID, string userID) {
        return _context.Likes.Any(l => l.PostID == postID && l.UserId == userID);
    }


    //returns the total number of likes for a specific post
    public int LikesForPost(int postID) {
        return _context.Likes.Where(l =>  l.PostID == postID).Count();
    }
}
