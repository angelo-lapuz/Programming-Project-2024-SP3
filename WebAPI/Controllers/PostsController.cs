using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Models.DataManager;

namespace WebAPI.Controllers;

// See here for more information:
// https://learn.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-7.0

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly PostManager _repo;

    public PostsController(PostManager repo)
    {
        _repo = repo;
    }

    // GET: api/posts
    // Returns a list of all posts
    [HttpGet]
    public IEnumerable<Post> Get()
    {
        return _repo.GetAll();
    }

    // GET api/posts/1
    // Returns a specific post by ID
    [HttpGet("{id}")]
    public Post Get(int id)
    {
        return _repo.Get(id);
    }

    // DELETE api/posts/1
    // Deletes a specific post by ID
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }

    // DELETE api/posts/DeleteAll
    // Deletes multiple posts based on a list of IDs
    [HttpDelete("DeleteAll")]
    public void DeleteAll(List<int> ids)
    {
        _repo.DeleteAll(ids);
    }

    // POST api/posts
    // Adds a new post to the collection
    [HttpPost]
    public void Post([FromBody] Post post)
    {
        _repo.Add(post);
    }

    // PUT api/posts
    // Updates an existing post
    [HttpPut]
    public void Put([FromBody] Post post)
    {
        _repo.Update(post.PostID, post);
    }

    // GET: api/posts/fromBoard/{boardID}/{userID}
    // retrieves posts from a specific board with pagination
    [HttpGet("fromBoard/{boardID}/{userID}")]
    public IActionResult GetBoardPosts(int boardID, string userID, int pageSize = 5, int pageIndex = 1)
    {
        var response = _repo.GetBoardPosts(boardID, userID, pageSize, pageIndex);

        if (response != null || response.Any()) return Ok(response);
        return BadRequest(new { Message = "Issue Fetching Posts" });
    }

}

