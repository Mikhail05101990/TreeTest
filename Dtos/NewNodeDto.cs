using System.ComponentModel.DataAnnotations;

namespace TreeTest.Dtos;

public class NewNodeDto
{
    [Required]
    public string treeName { get; set; } = "";
    [Required]
    public int parentNodeId { get; set; } = -1;
    [Required]
    public string nodeName { get; set; } = "";
}