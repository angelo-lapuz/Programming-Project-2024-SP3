﻿using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Models.DataManager;
using Task = WebAPI.Models.Task;

namespace WebApi.Controllers;

// See here for more information:
// https://learn.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-7.0

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TaskManager _repo;

    public TasksController(TaskManager repo)
    {
        _repo = repo;
    }

    // GET: api/tasks
    [HttpGet]
    public IEnumerable<Task> Get()
    {
        return _repo.GetAll();
    }

    // GET api/tasks/1
    [HttpGet("{id}")]
    public Task Get(int id)
    {
        return _repo.Get(id);
    }

    // POST api/tasks
    [HttpPost]
    public void Post([FromBody] Task task)
    {
        _repo.Add(task);
    }

    // PUT api/tasks
    [HttpPut]
    public void Put([FromBody] Task task)
    {
        _repo.Update(task.TaskID, task);
    }

    // DELETE api/tasks/1
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}

