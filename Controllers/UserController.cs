using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeTest.Data;
using TreeTest.Dtos;

namespace TreeTest.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly TreeContext _db;

    public UserController(ILogger<UserController> logger, TreeContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpPost("/api.user.journal.getRange")]
    public ActionResult<IEnumerable<RangeDto>> GetJournalEvents(RangeDto dto)
    {

        return new List<RangeDto>(){dto}.ToArray();
    }

    [HttpPost("/api.user.tree.get")]
    public ActionResult<Node> GetTree(string treeName)
    {
        try
        {
            Node? tree = _db.Nodes.Where(x => x.ParentId == 0 && x.Name.Equals(treeName)).FirstOrDefault();

            if(tree == null) 
            {
                tree = new Node()
                {
                    Name = treeName,
                    Children = new Node[0]
                };
                _db.Nodes.Add(tree);
                _db.SaveChanges();
            }

            return Ok(tree);
        }
        catch(Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
