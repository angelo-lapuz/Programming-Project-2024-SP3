using WebAPI.Data;
using WebAPI.Models.Repository;

namespace WebAPI.Models.DataManager;

public class BoardManager : IDataRepository<Board, int>
{
    private readonly PeakHubContext _context;

    public BoardManager(PeakHubContext context)
    {
        _context = context;
    }

    // gets a specific Board from the database by ID
    public Board Get(int id)
    {
        return _context.Boards.Find(id);
    }

    // gets all Boards from the database
    public IEnumerable<Board> GetAll()
    {
        return _context.Boards.ToList();
    }

    // gets the total number of Boards in the database
    public int GetBoardTotal() {
        return _context.Boards.Count();
    }

    // adds a new Board to the database
    public int Add(Board board)
    {
        _context.Boards.Add(board);
        _context.SaveChanges();
        return board.BoardID;
    }

    // deletes a specific Board from the database by ID
    public int Delete(int id)
    {
        _context.Boards.Remove(_context.Boards.Find(id));
        _context.SaveChanges();
        return id;
    }

    // deletes multiple Boards from the database based on a list of IDs
    public void DeleteAll(List<int> ids)
    {
        foreach (var id in ids)
        {
            _context.Boards.Remove(_context.Boards.Find(id));
        }
        _context.SaveChanges();
    }

    // updates an existing Board in the database
    public int Update(int id, Board board)
    {
        _context.Update(board);
        _context.SaveChanges();
        return id;
    }
}

