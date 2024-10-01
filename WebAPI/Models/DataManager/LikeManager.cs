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

    public int Update(int id, Like like)
    {
        _context.Update(like);
        _context.SaveChanges();

        return id;
    }
}
