using Microsoft.AspNetCore.Mvc;
using TreeTest.Data;
using TreeTest.BL;
using TreeTest.BL.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TreeTest.Controllers;

[ApiController]
[Route("")]
public class TreeController : ControllerBase
{
    private readonly ILogger<TreeController> _logger;
    private readonly TreeContext _db;
    private static JsonSerializerSettings jsnSettings = new JsonSerializerSettings()
    {
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
        Formatting = Formatting.Indented
    }; 

    public TreeController(ILogger<TreeController> logger, TreeContext db)
    {
        _logger = logger;
        _db = db;
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
                tree.ParentId = tree.Id;
                _db.SaveChanges();
            }
            else
            {
                List<Node> grandChildren = NodeMaster.FillChildrenReturnGrandChildren(tree, _db);
                
                while (grandChildren.Count > 0)
                {
                    List<Node> nodes = new List<Node>();

                    foreach(var child in grandChildren)
                        nodes.AddRange(NodeMaster.FillChildrenReturnGrandChildren(child, _db));
                    
                    grandChildren = nodes;
                }
            }

            return Ok(tree);
        }
        catch(Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}