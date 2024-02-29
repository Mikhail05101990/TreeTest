using System.ComponentModel.DataAnnotations;

namespace TreeTest.Dtos;

public class RenameNodeDto
{
    [Required]
    public string treeName { get; set; } = "";
    [Required]
    public int nodeId { get; set; }
    [Required]
    public string newNodeName { get; set; } = "";
}