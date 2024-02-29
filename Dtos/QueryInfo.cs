namespace TreeTest.Dtos;

public class QueryInfo
{
    public string Path { get; set; } = "";
    public string TreeName { get; set; } = "";
    public IQueryCollection? QueryParams { get; set; }
    public string Body { get; set; } = "";
}