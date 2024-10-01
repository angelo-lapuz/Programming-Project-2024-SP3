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

    public Peak Get(int id)
    {
        return _context.Peaks.Find(id);
    }

    public IEnumerable<Peak> GetAll()
    {
        return _context.Peaks.ToList();
    }

    public int Add(Peak peak)
    {
        _context.Peaks.Add(peak);
        _context.SaveChanges();

        return peak.PeakID;
    }

    public int Delete(int id)
    {
        _context.Peaks.Remove(_context.Peaks.Find(id));
        _context.SaveChanges();

        return id;
    }

    public int Update(int id, Peak peak)
    {
        _context.Update(peak);
        _context.SaveChanges();

        return id;
    }
}
