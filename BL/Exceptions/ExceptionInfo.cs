namespace TreeTest.BL.Exceptions;

public class ExceptionInfo
{
    public readonly string Type;
    public readonly Data Data;
    public readonly string Id;  

    public ExceptionInfo(string message, string eventId, string type)
    {
        Data = new Data(message);
        Id = eventId;
        Type = type;
    }
}