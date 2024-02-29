using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TreeTest.Dtos;

public class EventFilter
{
    public string From {get;set;} = "";
    public string To {get;set;} = "";
    public required string Search {get;set;} = "";

    private static string GetDateNow()
    {
        return DateTime.UtcNow.ToString();
    }
}