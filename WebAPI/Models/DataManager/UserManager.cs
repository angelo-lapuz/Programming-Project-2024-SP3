using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models.Repository;
using SimpleHashing.Net;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;


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

    // checks if the username and password are valid compared to what is stored in the database
    public User? VerifyLogin(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == username);

        if (user != null)
        {
            ISimpleHash simpleHash = new SimpleHash();
            bool isPasswordValid = simpleHash.Verify(password, user.Password);

            if (isPasswordValid)
            {
                return user;
            }
        }
        return null;
    }


    // checks if there is a username Or email in the database
    public User? GetByUsernameAndEmail(String username, String email)
    {
        return _context.Users.Where(u => u.UserName == username || u.Email == email).FirstOrDefault();
    }
}
