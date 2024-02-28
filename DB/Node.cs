namespace TreeTest.Data;
using System.ComponentModel.DataAnnotations.Schema;

public class Node
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int Id { get; set; } = 0;
    public string Name { get; set; } = "Root";
    public int ParentId { get; set; } = 0;
    public ICollection<Node> Children { get; set; } = new List<Node>();
}