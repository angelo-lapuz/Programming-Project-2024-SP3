using WebAPI.Data;
using WebAPI.Models.Repository;

namespace WebAPI.Models.DataManager;

public class TaskManager : IDataRepository<Peak, int>
{
    private readonly PeakHubContext _context;

    public TaskManager(PeakHubContext context)
    {
        _context = context;
    }

    public Peak Get(int id)
    {
        return _context.Tasks.Find(id);
    }

    public IEnumerable<Peak> GetAll()
    {
        return _context.Tasks.ToList();
    }

    public int Add(Peak task)
    {
        _context.Tasks.Add(task);
        _context.SaveChanges();

        return task.TaskID;
    }

    public int Delete(int id)
    {
        _context.Tasks.Remove(_context.Tasks.Find(id));
        _context.SaveChanges();

        return id;
    }

    public int Update(int id, Peak task)
    {
        _context.Update(task);
        _context.SaveChanges();

        return id;
    }
}
