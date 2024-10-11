using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Models.DataManager;

namespace WebAPI.Controllers;

// See here for more information:
// https://learn.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-7.0

[ApiController]
[Route("api/[controller]")]
public class LikesController : ControllerBase
{
    private readonly LikeManager _repo;

    public LikesController(LikeManager repo)
    {
        _repo = repo;
    }

    // GET: api/likes
    [HttpGet]
    public IEnumerable<Like> Get()
    {
        return _repo.GetAll();
    }

    // GET api/likes/1
    [HttpGet("{id}")]
    public Like Get(int id)
    {
        return _repo.Get(id);
    }

    // POST api/likes
    [HttpPost]
    public void Post([FromBody] Like like)
    {
        _repo.Add(like);
    }

    // PUT api/likes
    [HttpPut]
    public void Put([FromBody] Like like)
    {
        _repo.Update(like.LikeID, like);
    }

    // DELETE api/likes/1
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }

    // GET: api/likes/{postID}/{userID}
    [HttpGet("{postID}/{userID}")]
    public bool HasUserLikedPost(int postID, string userID) {
        return _repo.HasUserLikedPost(postID, userID);
    }

    // GET: api/likes/{postID}
    [HttpGet("posts/{postID}")]
    public int LikesForPost(int postID) {
        return _repo.LikesForPost(postID);
    }

    // Delete Like: api/likes/remove/{postID}/{userID}
    [HttpDelete("remove/{postID}/{userID}")]
    public bool RemoveExistingLike(int postID, string userID) {
        _repo.DeleteLike(postID, userID);
        return true;
    }

    // Add Like: api/likes/add/{postID}/{userID}
    [HttpPut("add/{postID}/{userID}")]
    public IActionResult LikeAPost(int postID, string userID) {
        _repo.LikePost(postID, userID);
        return Ok();
    }
}


