using System.ComponentModel.DataAnnotations;

namespace TreeTest.Dtos;

public class DeleteNodeDto
{
    [Required]
    public string treeName { get; set; } = "";
    [Required]
    public int nodeId { get; set; }
}