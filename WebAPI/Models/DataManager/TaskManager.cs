using WebAPI.Data;
using WebAPI.Models.Repository;

namespace WebAPI.Models.DataManager;

public class TaskManager : IDataRepository<Task, int>
{
    private readonly PeakHubContext _context;

    public TaskManager(PeakHubContext context)
    {
        _context = context;
    }

    public Task Get(int id)
    {
        return _context.Tasks.Find(id);
    }

    public IEnumerable<Task> GetAll()
    {
        return _context.Tasks.ToList();
    }

    public int Add(Task task)
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

    public int Update(int id, Task task)
    {
        _context.Update(task);
        _context.SaveChanges();

        return id;
    }
}
