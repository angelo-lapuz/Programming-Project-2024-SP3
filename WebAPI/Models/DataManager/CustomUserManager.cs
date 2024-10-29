using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models.Repository;

namespace WebAPI.Models.DataManager;

public class CustomUserManager : IDataRepository<User, int>
{
    private readonly PeakHubContext _context;
    private readonly ILogger<CustomUserManager> _logger;

    // Constructor with logger injected
    public CustomUserManager(PeakHubContext context, ILogger<CustomUserManager> logger)
    {
        _context = context;
        _logger = logger;
    }

    // gets a specific User from the database by ID
    public User Get(int id)
    {
        return _context.Users.Find(id);
    }

    // gets a specific User from the database by ID
    public User Get(string id)
    {
        return _context.Users.Find(id);
    }

    // gets all Users from the database
    public IEnumerable<User> GetAll()
    {
        return _context.Users.ToList();
    }

    // adds a new User to the database
    public int Add(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
        return 1;
    }

    // deletes a specific User from the database by ID
    public int Delete(int id)
    {
        _context.Users.Remove(_context.Users.Find(id));
        _context.SaveChanges();
        return id;
    }

    // deletes multiple Users from the database based on a list of IDs
    public void DeleteAll(List<string> ids)
    {
        foreach (var id in ids)
        {
            _context.Users.Remove(_context.Users.Find(id));
        }
        _context.SaveChanges();
    }

    // updates an existing User in the database
    public int Update(int id, User user)
    {
        _context.Update(user);
        _context.SaveChanges();
        return id;
    }

    // updates an existing User in the database
    public string Update(User user)
    {
        _context.Update(user);
        _context.SaveChanges();
        return user.Id;
    }

 
    // checks if there is a username Or email in the database - used when creating account
    public List<User> GetByUsernameAndEmail(String username, String email)
    {
        return _context.Users.Where(u => u.UserName == username || u.Email == email).Take(2).ToList();
    }

    public User getUserPosts(string id)
    {
        return _context.Users.Include(u => u.Posts).FirstOrDefault(u => u.Id == id);
    }

}