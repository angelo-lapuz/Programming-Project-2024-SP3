using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using WebAPI.Data;
using WebAPI.Models.Repository;

namespace WebAPI.Models.DataManager;

public class PeakManager : IDataRepository<Peak, int>
{
    private readonly PeakHubContext _context;

    public PeakManager(PeakHubContext context)
    {
        _context = context;
    }

    // gets a specific Peak from the database by ID
    public Peak Get(int id)
    {
        return _context.Peaks.Find(id);
    }

    //// get a specific peak by Name
    //public Peak Get(string name)
    //{
    //    return (Peak)_context.Peaks.Where(u => u.Name == name);
    //}

    // gets all Peaks from the database
    public IEnumerable<Peak> GetAll()
    {
        return _context.Peaks.ToList();
    }

    // adds a new Peak to the database
    public int Add(Peak peak)
    {
        _context.Peaks.Add(peak);
        _context.SaveChanges();
        return peak.PeakID;
    }

    // deletes a specific Peak from the database by ID
    public int Delete(int id)
    {
        _context.Peaks.Remove(_context.Peaks.Find(id));
        _context.SaveChanges();
        return id;
    }

    // deletes multiple Peaks from the database based on a list of IDs
    public void DeleteAll(List<int> ids)
    {
        foreach (var id in ids)
        {
            _context.Peaks.Remove(_context.Peaks.Find(id));
        }
        _context.SaveChanges();
    }

    // updates an existing Peak in the database
    public int Update(int id, Peak peak)
    {
        _context.Update(peak);
        _context.SaveChanges();
        return id;
    }
}
