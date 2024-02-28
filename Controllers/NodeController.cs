using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeTest.Data;
using TreeTest.Dtos;
using TreeTest.BL;

namespace TreeTest.Controllers;

[ApiController]
[Route("")]
public class NodeController : ControllerBase
{
    private readonly ILogger<NodeController> _logger;
    private readonly TreeContext _db;

    public NodeController(ILogger<NodeController> logger, TreeContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpPost("/api.user.tree.node.create")]
    public ActionResult<Node> CreateNode(NewNodeDto nnode)
    {
        var root = _db.Nodes.Where(x => x.ParentId == 0 && x.Name.Equals(nnode.treeName)).FirstOrDefault();
        var node = _db.Nodes.Where(x => x.Id.Equals(nnode.parentNodeId)).FirstOrDefault();
        int errType = 0;

        if(root == null )
            errType += 1;
        if(node == null)
            errType += 2;
        
        if(errType > 0)
        {
            if(errType == 1)
                return StatusCode(500, "Requested node was found, but it doesn't belong your tree");
            else 
                return StatusCode(500, $"Node with ID = {nnode.parentNodeId} was not found");
        }

        var nodesWithTheSameName = _db.Nodes.Where(x => x.ParentId == node.ParentId).ToList();
        var duplicate = nodesWithTheSameName.Where(x => x.Name.Equals(nnode.nodeName));
        
        if(duplicate != null)
        {
            node.Children.Add(new Node(){
                Name = nnode.nodeName,
                ParentId = nnode.parentNodeId
            });
            
            _db.SaveChanges();

            return StatusCode(200);
        }
        else
        {
            return StatusCode(500, $"Duplicate name");
        }        
    }

    [HttpPost("/api.user.tree.node.delete")]
    public ActionResult DeleteNode(DeleteNodeDto node)
    {
        var n = _db.Nodes.Where(x => x.Id.Equals(node.nodeId)).FirstOrDefault();

        if(n != null)
        {
            var children = _db.Nodes.Where(x => x.ParentId == node.nodeId).ToList();
            
            if(children.Count > 0)
                return StatusCode(500, $"You have to delete all children nodes first");
            else
            {
                _db.Nodes.Remove(n);
                _db.SaveChanges();
            }
        }
        else
            return StatusCode(500, $"Node with ID = {node.nodeId} was not found");

        return Ok();
    }

    [HttpPost("/api.user.tree.node.rename")]
    public ActionResult RenameNode(RenameNodeDto node)
    {
        Node? tree = _db.Nodes.Where(x => x.ParentId == 0 && x.Name.Equals(node.treeName)).FirstOrDefault();
        Node? curNode = _db.Nodes.Where(x => x.Id == node.nodeId).FirstOrDefault();

        if(curNode != null)
        {
            if(tree == null) 
                return StatusCode(500, $"Requested node was found, but it doesn't belong your tree");
               
            else
            {
                if(curNode.Id == tree.Id)
                    return StatusCode(500, $"Couldn't rename root node");

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

                return StatusCode(500, "Requested node was found, but it doesn't belong your tree");
            }
        }
        else
            return StatusCode(500, $"Node with ID = {node.nodeId} was not found");
    }
}
