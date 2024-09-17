using WebAPI.Data;
using WebAPI.Models.Repository;

namespace WebAPI.Models.DataManager;

public class UserManager : IDataRepository<User, int>
{
    private readonly PeakHubContext _context;

    public UserManager(PeakHubContext context)
    {
        _context = context;
    }

    public User Get(int id)
    {
        return _context.Users.Find(id);
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users.ToList();
    }

    public int Add(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();

        return user.UserID;
    }

    public int Delete(int id)
    {
        _context.Users.Remove(_context.Users.Find(id));
        _context.SaveChanges();

        return id;
    }

    public int Update(int id, User user)
    {
        _context.Update(user);
        _context.SaveChanges();

        return id;
    }

    // Adam's Additions [For Sign Up Checking]
    public User GetByUsername(string username) {
        return _context.Users.FirstOrDefault(u => u.UserName == username);
    }
    public User GetByEmail(string email) {
        return _context.Users.FirstOrDefault(u => u.Email == email);
    }
}
