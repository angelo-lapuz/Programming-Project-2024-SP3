using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Models.DataManager;


namespace WebApi.Controllers;

// See here for more information:
// https://learn.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-7.0

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase {
    private readonly UserManager _repo;
    private readonly ILogger<UsersController> _logger;
    
    public UsersController(UserManager repo, ILogger<UsersController> logger) {
        _repo = repo;
        _logger = logger;
    }

    // GET api/users/1
    [HttpGet("{id:int}")]
    public User Get(int id) { return _repo.Get(id); }

    // POST api/users
    [HttpPost]
    public void Post([FromBody] User user) { _repo.Add(user); }

    // PUT api/users
    [HttpPut]
    public void Put([FromBody] User user) {
        _repo.Update(user.UserID, user);
    }

    // DELETE api/users/1
    [HttpDelete("{id:int}")]
    public long Delete(int id) { return _repo.Delete(id); }

    // when to check if a username or email exists in the database
    [HttpGet("Verify/{username}/{email}")]
    public IActionResult Verify(string username, string email) {
        // get username / email
        List<User> users = _repo.GetByUsernameAndEmail(username, email);
        bool nameTaken = false, emailTaken = false;

        // Adam -- Sort Data
        if (users.Count == 2) {
            nameTaken = true;
            emailTaken = true;
        }
        else if (users.Count == 1) {
            User user = users[0];
            if (user.UserName == username) nameTaken = true;
            if (user.Email == email) emailTaken = true;
        }

        // return new JSON result with the username and email 
        return Ok(new {
            UsernameExists = nameTaken, EmailExists = emailTaken
        });
    }

    // used when logging in
    [HttpGet("{username}/{password}")]
    public User Login(string username, string password) {
        return _repo.VerifyLogin(username, password);
    }
}

