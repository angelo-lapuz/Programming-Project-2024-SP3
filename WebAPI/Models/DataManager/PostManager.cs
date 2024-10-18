using WebAPI.Data;
using WebAPI.Models.Repository;
using WebAPI.ViewModels;

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

    // LazyLoad + Pagination
    public IEnumerable<PostViewModel> GetBoardPosts(int boardID, string userID, int pageSize, int pageIndex) {
        return _context.Posts
            .Where(p => p.BoardID == boardID)
            .Select(p => new PostViewModel {
                PostID = p.PostID,
                Content = p.Content,
                Media = p.MediaLink,
                LikeCount = p.Likes.Count(),
                TransactionTimeUTC = p.TransactionTimeUtc,
                HasUserLiked = p.Likes.Any(l => l.User.Id == userID),
                User = new UserViewModel {
                    UserID = p.User.Id,
                    Username = p.User.UserName,
                    ProfileImg = p.User.ProfileIMG ?? "https://peakhub-user-content.s3.amazonaws.com/default.jpg"
                }
            })
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
}
