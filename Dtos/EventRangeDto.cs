using System.ComponentModel.DataAnnotations;

namespace TreeTest.Dtos;

public class EventRangeDto
{
    [Required]
    public required int Skip {get;set;}
    [Required]
    public required int Take {get;set;}
}