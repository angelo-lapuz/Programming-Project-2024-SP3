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

    public Award Get(int id)
    {
        return _context.Awards.Find(id);
    }

    public IEnumerable<Award> GetAll()
    {
        return _context.Awards.ToList();
    }

    public int Add(Award award)
    {
        _context.Awards.Add(award);
        _context.SaveChanges();

        return award.AwardID;
    }

    public int Delete(int id)
    {
        _context.Awards.Remove(_context.Awards.Find(id));
        _context.SaveChanges();

        return id;
    }

    public int Update(int id, Award award)
    {
        _context.Update(award);
        _context.SaveChanges();

        return id;
    }
}

