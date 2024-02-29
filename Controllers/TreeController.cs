using Microsoft.AspNetCore.Mvc;
using TreeTest.Data;
using TreeTest.BL;
using TreeTest.BL.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TreeTest.BL.Tools;
using TreeTest.Dtos;
using System.ComponentModel.DataAnnotations;

namespace TreeTest.Controllers;

/// <summary>
/// Represents tree API
/// </summary>
[Tags("user.tree")]
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

    /// <remarks>Returns your entire tree. If your tree doesn't exist it will be created automatically.</remarks>
    [HttpPost("/api.user.tree.get")]
    public async Task<ActionResult<Node>> GetTree([Required]string treeName)
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
            ExceptionInfo exInfo = await ProcessException(e);
            
            return StatusCode(500, JsonConvert.SerializeObject(exInfo, jsnSettings));
        }
    }

    private async Task<ExceptionInfo> ProcessException(Exception e)
    {
        RequestInfoExctractor exctractor = new RequestInfoExctractor();
        QueryInfo queryInfo = await exctractor.Get(Request); 
        long eventId = ExceptionWriter.Save(_db, queryInfo, e);

        return new ExceptionInfo($"Internal server error ID = {eventId}.", eventId.ToString(), e.GetType().Name);
    }
}