using WebAPI.Data;
using WebAPI.Models.Repository;

namespace WebAPI.Models.DataManager;

public class AwardManager : IDataRepository<Award, int>
{
    private readonly PeakHubContext _context;

    public AwardManager(PeakHubContext context)
    {
        _context = context;
    }

    // gets a specific Award from the database by ID
    public Award Get(int id)
    {
        return _context.Awards.Find(id);
    }

    // gets all Awards from the database
    public IEnumerable<Award> GetAll()
    {
        return _context.Awards.ToList();
    }

    // adds a new Award to the database
    public int Add(Award award)
    {
        _context.Awards.Add(award);
        _context.SaveChanges();
        return award.AwardID;
    }

    // deletes a specific Award from the database by ID
    public int Delete(int id)
    {
        _context.Awards.Remove(_context.Awards.Find(id));
        _context.SaveChanges();
        return id;
    }

    // deletes multiple Awards from the database based on a list of IDs
    public void DeleteAll(List<int> ids)
    {
        foreach (var id in ids)
        {
            _context.Awards.Remove(_context.Awards.Find(id));
        }
        _context.SaveChanges();
    }

    // updates an existing Award in the database
    public int Update(int id, Award award)
    {
        _context.Update(award);
        _context.SaveChanges();
        return id;
    }
}

