﻿using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Models.DataManager;

namespace WebApi.Controllers;

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
    [HttpGet]
    public IEnumerable<Post> Get()
    {
        return _repo.GetAll();
    }

    // GET api/posts/1
    [HttpGet("{id}")]
    public Post Get(int id)
    {
        return _repo.Get(id);
    }

    // POST api/posts
    [HttpPost]
    public void Post([FromBody] Post post)
    {
        _repo.Add(post);
    }

    // PUT api/posts
    [HttpPut]
    public void Put([FromBody] Post post)
    {
        _repo.Update(post.PostID, post);
    }

    // DELETE api/posts/1
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}
