using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Models.DataManager;

namespace WebApi.Controllers;

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
    [HttpGet]
    public IEnumerable<Award> Get()
    {
        return _repo.GetAll();
    }

    // GET api/awards/1
    [HttpGet("{id}")]
    public Award Get(int id)
    {
        return _repo.Get(id);
    }

    // POST api/awards
    [HttpPost]
    public void Post([FromBody] Award award)
    {
        _repo.Add(award);
    }

    // PUT api/awards
    [HttpPut]
    public void Put([FromBody] Award award)
    {
        _repo.Update(award.AwardID, award);
    }

    // DELETE api/awards/1
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}




