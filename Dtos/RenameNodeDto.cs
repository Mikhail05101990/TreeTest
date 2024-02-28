namespace TreeTest.Dtos;

public class RenameNodeDto
{
    public string treeName { get; set; } = "";
    public int nodeId { get; set; }
    public string newNodeName { get; set; } = "";
}