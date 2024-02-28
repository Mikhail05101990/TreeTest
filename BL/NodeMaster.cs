using Microsoft.EntityFrameworkCore;
using TreeTest.Data;

namespace TreeTest.BL;

public class NodeMaster
{
    public static List<Node> FillChildrenReturnGrandChildren(Node node, TreeContext db)
    {
        var children = db.Nodes.Where(x => x.ParentId == node.Id).ToList();
        
        node.Children = children == null ? new List<Node>() : children.ToArray();

        if(children != null)
        {
            List<Node> grandChildren = new List<Node>();

            foreach(var child in children)
            {
                var items = db.Nodes.Where(x => x.ParentId == child.Id).ToList();
                grandChildren.AddRange(items);
            }

            return grandChildren;
        }
        else
            return new List<Node>();
    }
}