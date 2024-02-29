namespace TreeTest.BL.Exceptions;

public class SecureException: Exception
{
    public readonly string type = "Secure";

    public SecureException(string message): base(message)
    {
    }
}