using WebAPI.Data;
using WebAPI.Models.Repository;

namespace WebAPI.Models.DataManager;

public class PostManager : IDataRepository<Post, int>
{
    private readonly PeakHubContext _context;

    public PostManager(PeakHubContext context)
    {
        _context = context;
    }

    public Post Get(int id)
    {
        return _context.Posts.Find(id);
    }

    public IEnumerable<Post> GetAll()
    {
        return _context.Posts.ToList();
    }

    public int Add(Post post)
    {
        _context.Posts.Add(post);
        _context.SaveChanges();

        return post.PostID;
    }

    public int Delete(int id)
    {
        _context.Posts.Remove(_context.Posts.Find(id));
        _context.SaveChanges();

        return id;
    }

    public int Update(int id, Post post)
    {
        _context.Update(post);
        _context.SaveChanges();

        return id;
    }
}
