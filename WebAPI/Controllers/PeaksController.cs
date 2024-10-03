using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Models.DataManager;
using Peak = WebAPI.Models.Peak;

namespace WebApi.Controllers;

// See here for more information:
// https://learn.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-7.0

[ApiController]
[Route("api/[controller]")]
public class PeaksController : ControllerBase
{
    private readonly PeakManager _repo;

    public PeaksController(PeakManager repo)
    {
        _repo = repo;
    }

    // GET: api/peaks
    [HttpGet]
    public IEnumerable<Peak> Get()
    {
        return _repo.GetAll();
    }

    // GET api/peaks/1
    [HttpGet("{id}")]
    public Peak Get(int id)
    {
        return _repo.Get(id);
    }

    // POST api/peaks
    [HttpPost]
    public void Post([FromBody] Peak peak)
    {
        _repo.Add(peak);
    }

    // PUT api/peaks
    [HttpPut]
    public void Put([FromBody] Peak peak)
    {
        _repo.Update(peak.PeakID, peak);
    }

    // DELETE api/peaks/1
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}

