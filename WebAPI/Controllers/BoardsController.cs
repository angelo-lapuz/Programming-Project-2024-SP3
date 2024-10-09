using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Models.DataManager;

namespace WebAPI.Controllers;

// See here for more information:
// https://learn.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-7.0

[ApiController]
[Route("api/[controller]")]
public class BoardsController : ControllerBase
{
    private readonly BoardManager _repo;

    public BoardsController(BoardManager repo)
    {
        _repo = repo;
    }

    // GET: api/boards
    [HttpGet]
    public IEnumerable<Board> Get()
    {
        return _repo.GetAll();
    }

    // GET api/boards/1
    [HttpGet("{id}")]
    public Board Get(int id)
    {
        return _repo.Get(id);
    }

    // GET api/boards/total
    [HttpGet("total")]
    public int GetBoardTotal() {
        return _repo.GetBoardTotal();
    }

    // POST api/boards
    [HttpPost]
    public void Post([FromBody] Board board)
    {
        _repo.Add(board);
    }

    // PUT api/boards
    [HttpPut]
    public void Put([FromBody] Board board)
    {
        _repo.Update(board.BoardID, board);
    }

    // DELETE api/boards/1
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}



