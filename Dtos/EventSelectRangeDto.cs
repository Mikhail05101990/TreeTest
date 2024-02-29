namespace TreeTest.Dtos;

public class EventSelectRangeDto
{
    public int Skip { get; set; }
    public int Count { get; set; }
    public EventInfoShort[] Items { get; set; } = new EventInfoShort[0];
}