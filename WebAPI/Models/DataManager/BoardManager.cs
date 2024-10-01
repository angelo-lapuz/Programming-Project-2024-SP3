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

    public Board Get(int id)
    {
        return _context.Boards.Find(id);
    }

    public IEnumerable<Board> GetAll()
    {
        return _context.Boards.ToList();
    }

    public int Add(Board board)
    {
        _context.Boards.Add(board);
        _context.SaveChanges();

        return board.BoardID;
    }

    public int Delete(int id)
    {
        _context.Boards.Remove(_context.Boards.Find(id));
        _context.SaveChanges();

        return id;
    }

    public int Update(int id, Board board)
    {
        _context.Update(board);
        _context.SaveChanges();

        return id;
    }
}

