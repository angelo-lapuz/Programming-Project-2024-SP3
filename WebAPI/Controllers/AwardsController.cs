using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Models.DataManager;

namespace WebAPI.Controllers;

// See here for more information:
// https://learn.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-7.0

[ApiController]
[Route("api/[controller]")]
public class AwardsController : ControllerBase
{
    private readonly AwardManager _repo;

    public AwardsController(AwardManager repo)
    {
        _repo = repo;
    }

    // GET: api/awards
    // Returns a list of all awards
    [HttpGet]
    public IEnumerable<Award> Get()
    {
        return _repo.GetAll();
    }

    // GET api/awards/1
    // Returns a specific award by ID
    [HttpGet("{id}")]
    public Award Get(int id)
    {
        return _repo.Get(id);
    }

    // POST api/awards
    // Adds a new award to the collection
    [HttpPost]
    public void Post([FromBody] Award award)
    {
        _repo.Add(award);
    }

    // PUT api/awards
    // Updates an existing award
    [HttpPut]
    public void Put([FromBody] Award award)
    {
        _repo.Update(award.AwardID, award);
    }

    // DELETE api/awards/1
    // Deletes an award by its ID
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }

    // DELETE api/awards/DeleteAll
    // Deletes multiple awards based on a list of IDs
    [HttpDelete("DeleteAll")]
    public void DeleteAll(List<int> ids)
    {
        _repo.DeleteAll(ids);
    }
}
