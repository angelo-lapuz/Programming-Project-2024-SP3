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
    // Returns a list of all likes
    [HttpGet]
    public IEnumerable<Like> Get()
    {
        return _repo.GetAll();
    }

    // GET api/likes/1
    // Returns a specific like by ID
    [HttpGet("{id}")]
    public Like Get(int id)
    {
        return _repo.Get(id);
    }

    // POST api/likes
    // Adds a new like to the collection
    [HttpPost]
    public void Post([FromBody] Like like)
    {
        _repo.Add(like);
    }

    // PUT api/likes
    // Updates an existing like
    [HttpPut]
    public void Put([FromBody] Like like)
    {
        _repo.Update(like.LikeID, like);
    }

    // DELETE api/likes/1
    // Deletes a specific like by ID
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }

    // DELETE api/likes/DeleteAll
    // Deletes multiple likes based on a list of IDs
    [HttpDelete("DeleteAll")]
    public void DeleteAll(List<int> ids)
    {
        _repo.DeleteAll(ids);
    }

    // GET: api/likes/{postID}/{userID}
    // Checks if a user has liked a specific post
    [HttpGet("{postID}/{userID}")]
    public bool HasUserLikedPost(int postID, string userID)
    {
        return _repo.HasUserLikedPost(postID, userID);
    }

    // GET: api/likes/posts/{postID}
    // Returns the total number of likes for a specific post
    [HttpGet("posts/{postID}")]
    public int LikesForPost(int postID)
    {
        return _repo.LikesForPost(postID);
    }

    // DELETE: api/likes/remove/{postID}/{userID}
    // Removes an existing like for a specific post by a user
    [HttpDelete("remove/{postID}/{userID}")]
    public bool RemoveExistingLike(int postID, string userID)
    {
        _repo.DeleteLike(postID, userID);
        return true;
    }

    // PUT: api/likes/add/{postID}/{userID}
    // Adds a like to a specific post by a user
    [HttpPut("add/{postID}/{userID}")]
    public IActionResult LikeAPost(int postID, string userID)
    {
        _repo.LikePost(postID, userID);
        return Ok();
    }
}
