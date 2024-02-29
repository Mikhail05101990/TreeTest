namespace TreeTest.Dtos;

public class EventInfoShort
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.MinValue;
}