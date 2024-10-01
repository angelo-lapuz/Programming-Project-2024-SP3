using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<User> _userManager;

    public UsersController(UserManager repo, ILogger<UsersController> logger, UserManager<User> userManager) {
        _repo = repo;
        _logger = logger;
        _userManager = userManager;
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
        _repo.Update(user.Id, user);
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

    // GET api/users/confirmemail
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            return BadRequest("User ID and token are required.");
        }

        var user = await _userManager.FindByNameAsync(userId);
        // if user hasn't been found 
        if (user == null) 
        {
            return NotFound($"User with ID '{userId}' not found.");
        }
        // check if email has been verified / confirmed
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded) 
        {
            _logger.LogInformation("User {UserName} confirmed their email.", user.UserName);
            return Ok("Email confirmed successfully.");
        }

        _logger.LogWarning("Email confirmation failed for {UserName}.", user.UserName);
        return BadRequest("Email confirmation failed");
    }


    //// used when logging in
    //[HttpGet("{username}/{password}")]
    //public User Login(string username, string password) {
    //    return _repo.VerifyLogin(username, password);
    //}
}

