using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.DataManager;
using Peak = WebAPI.Models.Peak;

namespace WebAPI.Controllers;

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
    // Returns a list of all peaks
    [HttpGet]
    public IEnumerable<Peak> Get()
    {
        return _repo.GetAll();
    }

    // GET api/peaks/1
    // Returns a specific peak by ID
    [HttpGet("{id}")]
    public Peak Get(int id)
    {
        return _repo.Get(id);
    }

    //// GET api/peaks/1
    //// Returns a specific peak by name
    //[HttpGet("GetPeak/{peakName}")]
    //public Peak GetPeak(string peakName)
    //{
    //    return _repo.Get(peakName);
    //}

    // POST api/peaks
    // Adds a new peak to the collection
    [HttpPost]
    public void Post([FromBody] Peak peak)
    {
        _repo.Add(peak);
    }

    // PUT api/peaks
    // Updates an existing peak
    [HttpPut]
    public void Put([FromBody] Peak peak)
    {
        _repo.Update(peak.PeakID, peak);
    }

    // DELETE api/peaks/1
    // Deletes a specific peak by ID
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }

    // DELETE api/peaks/DeleteAll
    // Deletes multiple peaks based on a list of IDs
    [HttpDelete("DeleteAll")]
    public void DeleteAll(List<int> ids)
    {
        _repo.DeleteAll(ids);
    }
}
