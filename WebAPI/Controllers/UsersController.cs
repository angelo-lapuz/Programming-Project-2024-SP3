using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Models.DataManager;

namespace WebApi.Controllers;

// See here for more information:
// https://learn.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-7.0

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager _repo;

    public UsersController(UserManager repo)
    {
        _repo = repo;
    }

    // GET: api/users
    [HttpGet]
    public IEnumerable<User> Get()
    {
        return _repo.GetAll();
    }

    // GET api/users/1
    [HttpGet("{id}")]
    public User Get(int id)
    {
        return _repo.Get(id);
    }

    // POST api/users
    [HttpPost]
    public void Post([FromBody] User user)
    {
        _repo.Add(user);
    }

    // PUT api/users
    [HttpPut]
    public void Put([FromBody] User user)
    {
        _repo.Update(user.UserID, user);
    }

    // DELETE api/users/1
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }

    // Adam's Additions [For Sign Up Checking]
    [HttpGet("username/{username}")]
    public ActionResult<User> GetByUsername(string username) {
        var user = _repo.GetByUsername(username);
        if (user == null) { return NotFound(); }
        return Ok(user);
    }

    [HttpGet("email/{email}")]
    public ActionResult<User> GetByEmail(string email) {
        var user = _repo.GetByEmail(email);
        if (user == null) { return NotFound(); }
        return Ok(user); 
    }
}

