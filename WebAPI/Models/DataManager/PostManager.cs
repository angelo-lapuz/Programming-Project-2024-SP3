using WebAPI.Data;
using WebAPI.Models.Repository;
using WebAPI.ViewModels;
using X.PagedList;
using X.PagedList.Extensions;

namespace WebAPI.Models.DataManager;

public class PostManager : IDataRepository<Post, int>
{
    private readonly PeakHubContext _context;

    public PostManager(PeakHubContext context)
    {
        _context = context;
    }

    // gets a specific Post from the database by ID
    public Post Get(int id)
    {
        return _context.Posts.Find(id);
    }

    // gets all Posts from the database
    public IEnumerable<Post> GetAll()
    {
        return _context.Posts.ToList();
    }

    // adds a new Post to the database
    public int Add(Post post)
    {
        _context.Posts.Add(post);
        _context.SaveChanges();
        return post.PostID;
    }

    // deletes a specific Post from the database by ID
    public int Delete(int id)
    {
        _context.Posts.Remove(_context.Posts.Find(id));
        _context.SaveChanges();
        return id;
    }

    // deletes multiple Posts from the database based on a list of IDs
    public void DeleteAll(List<int> ids)
    {
        foreach (var id in ids)
        {
            _context.Posts.Remove(_context.Posts.Find(id));
        }
        _context.SaveChanges();
    }

    // updates an existing Post in the database
    public int Update(int id, Post post)
    {
        _context.Update(post);
        _context.SaveChanges();
        return id;
    }

    // gets 'x' posts from the database based on a board ID, user ID -- pagination and lazy loading
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

    public IEnumerable<PostViewModel> GetUserPosts(int boardID, string userID, int pageSize, int pageIndex)
    {
        return _context.Posts
            .Where(p => p.BoardID == boardID)
            .Select(p => new PostViewModel
            {
                PostID = p.PostID,
                Content = p.Content,
                Media = p.MediaLink,
                LikeCount = p.Likes.Count(),
                TransactionTimeUTC = p.TransactionTimeUtc,
                HasUserLiked = p.Likes.Any(l => l.User.Id == userID),
                User = new UserViewModel
                {
                    UserID = p.User.Id,
                    Username = p.User.UserName,
                    ProfileImg = p.User.ProfileIMG ?? "https://peakhub-user-content.s3.amazonaws.com/default.jpg"
                }
            })
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    // uses x.PageList to get number of posts based on page number
    public IPagedList<Post> GetPagedList(string userID, int page = 1)
    {
        const int pageSize = 5;
        return _context.Posts
            .Where(x => x.UserId == userID)
            .OrderBy(x => x.TransactionTimeUtc)
            .ToPagedList(page, pageSize);
    }
}
