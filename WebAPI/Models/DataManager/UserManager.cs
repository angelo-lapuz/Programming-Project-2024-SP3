using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models.Repository;
using SimpleHashing.Net;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Models.DataManager;

public class UserManager : IDataRepository<User, int>
{
    private readonly PeakHubContext _context;
    private readonly ILogger<UserManager> _logger;

    // Constructor with logger injected
    public UserManager(PeakHubContext context, ILogger<UserManager> logger)
    {
        _context = context;
        _logger = logger;
    }

    public User Get(int id)
    {
        return _context.Users.Find(id);
    }

    public User Get(string id)
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
        return 1;
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

    public string Update(string id, User user)
    {
        _context.Update(user);
        _context.SaveChanges();
        return user.Id;
    }

    public User getUser(string id)
    {

        return _context.Users
                    .Include(u => u.Posts)
                    .ThenInclude(p => p.Likes)
                    .Include(u => u.Awards)
                    .Include(u => u.Peaks)
                    .AsSplitQuery()
                    .FirstOrDefault(u => u.Id == id);
    }

    // checks if there is a username Or email in the database
    public List<User> GetByUsernameAndEmail(String username, String email)
    {
        return _context.Users.Where(u => u.UserName == username || u.Email == email).Take(2).ToList();
    }

}