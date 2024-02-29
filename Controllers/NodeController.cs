using Microsoft.AspNetCore.Mvc;
using TreeTest.Data;
using TreeTest.Dtos;
using TreeTest.BL;
using TreeTest.BL.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TreeTest.BL.Tools;

namespace TreeTest.Controllers;

/// <summary>
/// Represents tree node API
/// </summary>
[Tags("user.tree.node")]
[ApiController]
[Route("")]
public class NodeController : ControllerBase
{
    private readonly ILogger<NodeController> _logger;
    private readonly TreeContext _db;
    private static JsonSerializerSettings jsnSettings = new JsonSerializerSettings()
    {
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
        Formatting = Formatting.Indented
    }; 

    public NodeController(ILogger<NodeController> logger, TreeContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <remarks>Create a new node in your tree. You must to specify a parent node ID that belongs to your tree. A new node name must be unique across all siblings.</remarks>
    [HttpPost("/api.user.tree.node.create")]
    public async Task<IActionResult> CreateNode([FromQuery]NewNodeDto nnode)
    {
        try
        {
            var root = _db.Nodes.Where(x => x.ParentId == 0 && x.Name.Equals(nnode.treeName)).FirstOrDefault();
            var node = _db.Nodes.Where(x => x.Id.Equals(nnode.parentNodeId)).FirstOrDefault();

            if(node == null)
                throw new SecureException($"Node with ID = {nnode.parentNodeId} was not found");
            
            if(root == null)
                throw new SecureException("Requested node was found, but it doesn't belong your tree");

            List<Node> nodesWithTheSameName = _db.Nodes.Where(x => x.ParentId == node.ParentId).ToList();
            Node? duplicate = nodesWithTheSameName.Where(x => x.Name.Equals(nnode.nodeName)).FirstOrDefault();

            if(duplicate == null)
            {
                node.Children.Add(new Node(){
                    Name = nnode.nodeName,
                    ParentId = nnode.parentNodeId
                });
            
                _db.SaveChanges();

                return StatusCode(200);
            }
            else
                throw new SecureException("Duplicate name");  
            
        }
        catch(SecureException se)
        {
            ExceptionInfo exInfo = await ProcessSecureException(se, nnode.treeName);

            return StatusCode(500, JsonConvert.SerializeObject(exInfo, jsnSettings));
        }
        catch(Exception e)
        {
            ExceptionInfo exInfo = await ProcessException(e);
            
            return StatusCode(500, JsonConvert.SerializeObject(exInfo, jsnSettings));
        }
    }

    /// <remarks>Delete an existing node in your tree. You must specify a node ID that belongs your tree.</remarks>
    [HttpPost("/api.user.tree.node.delete")]
    public async Task<ActionResult> DeleteNode([FromQuery]DeleteNodeDto node)
    {
        try
        {
            var n = _db.Nodes.Where(x => x.Id.Equals(node.nodeId)).FirstOrDefault();

            if(n != null)
            {
                var children = _db.Nodes.Where(x => x.ParentId == node.nodeId).ToList();
                
                if(children.Count > 0)
                    throw new SecureException($"You have to delete all children nodes first");
                else
                {
                    _db.Nodes.Remove(n);
                    _db.SaveChanges();
                }
            }
            else
                throw new SecureException($"Node with ID = {node.nodeId} was not found");

            return Ok();
        }
        catch(SecureException se)
        {
            ExceptionInfo exInfo = await ProcessSecureException(se, node.treeName);

            return StatusCode(500, JsonConvert.SerializeObject(exInfo, jsnSettings));
        }
        catch(Exception e)
        {
            ExceptionInfo exInfo = await ProcessException(e);
            
            return StatusCode(500, JsonConvert.SerializeObject(exInfo, jsnSettings));
        }        
    }

    /// <remarks>Rename an existing node in your tree. You must specify a node ID that belongs your tree. A new name of the node must be unique across all siblings.</remarks>
    [HttpPost("/api.user.tree.node.rename")]
    public async Task<ActionResult> RenameNode([FromQuery]RenameNodeDto node)
    {
        try
        {
            Node? tree = _db.Nodes.Where(x => x.ParentId == 0 && x.Name.Equals(node.treeName)).FirstOrDefault();
            Node? curNode = _db.Nodes.Where(x => x.Id == node.nodeId).FirstOrDefault();

            if(curNode != null)
            {
                if(tree == null) 
                    throw new SecureException($"Requested node was found, but it doesn't belong your tree");
                
                else
                {
                    if(curNode.Id == tree.Id)
                        throw new SecureException($"Couldn't rename root node");

                    List<Node> grandChildren = NodeMaster.FillChildrenReturnGrandChildren(tree, _db);
                    
                    foreach(var child in tree.Children)
                    {
                        if(child.Id == node.nodeId)
                        {
                            child.Name = node.newNodeName;
                            _db.SaveChanges();

                            return Ok();
                        }
                    }

                    while (grandChildren.Count > 0)
                    {
                        List<Node> nodes = new List<Node>();

                        foreach(var child in grandChildren)
                        {
                            if(child.Id == node.nodeId)
                            {
                                child.Name = node.newNodeName;
                                _db.SaveChanges();

                                return Ok();
                            }
                                
                            nodes.AddRange(NodeMaster.FillChildrenReturnGrandChildren(child, _db));
                        }
                            
                        grandChildren = nodes;
                    }

                    throw new SecureException("Requested node was found, but it doesn't belong your tree");
                }
            }
            else
                throw new SecureException($"Node with ID = {node.nodeId} was not found");
        }
        catch(SecureException se)
        {
            ExceptionInfo exInfo = await ProcessSecureException(se, node.treeName);
            
            return StatusCode(500, JsonConvert.SerializeObject(exInfo, jsnSettings));
        }
        catch(Exception e)
        {
            ExceptionInfo exInfo = await ProcessException(e);
            
            return StatusCode(500, JsonConvert.SerializeObject(exInfo, jsnSettings));
        }
    }

    private async Task<ExceptionInfo> ProcessSecureException(SecureException se, string nodeName)
    {
        RequestInfoExctractor exctractor = new RequestInfoExctractor();
        QueryInfo queryInfo = await exctractor.Get(Request); 
        queryInfo.TreeName = nodeName;
        long eventId = ExceptionWriter.SaveSecure(_db, queryInfo, se);
        string eventType = se.GetType().Name.Replace("Exception", "");
            
        return new ExceptionInfo(se.Message, eventId.ToString(), eventType);
    }

    private async Task<ExceptionInfo> ProcessException(Exception e)
    {
        RequestInfoExctractor exctractor = new RequestInfoExctractor();
        QueryInfo queryInfo = await exctractor.Get(Request); 
        long eventId = ExceptionWriter.Save(_db, queryInfo, e);

        return new ExceptionInfo(e.Message, eventId.ToString(), e.GetType().Name);
    }
}
