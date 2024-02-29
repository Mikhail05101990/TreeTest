using System.Text;
using TreeTest.BL.Exceptions;
using TreeTest.Dtos;

namespace TreeTest.BL.Tools;

public class ExceptionInfoFormatter
{
    public static string GetPlainText(SecureException se, QueryInfo queryInfo, long eventId)
    {
        StringBuilder sb = new StringBuilder($"Request ID = {eventId}\r\n");
        sb.Append($"Path = {queryInfo.Path}\r\n");
        
        if(queryInfo.QueryParams != null)
            foreach(var item in queryInfo.QueryParams)
                sb.Append($"{item.Key} = {item.Value}\r\n");
        
        sb.Append("\r\n");
        
        if(!string.IsNullOrEmpty(queryInfo.Body))
            sb.Append($"{queryInfo.Body}\r\n");
        
        sb.Append($"\r\n{se.Message}\r\n");
        sb.Append($"{se.StackTrace}");
    
        return sb.ToString();
    }
}