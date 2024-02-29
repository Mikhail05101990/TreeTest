using System.Text;
using TreeTest.Dtos;

namespace TreeTest.BL.Tools;

public class ExceptionInfoFormatter
{
    public static string GetPlainText(object exObj, QueryInfo queryInfo, long eventId)
    {
        Exception ex = (Exception)exObj;
        StringBuilder sb = new StringBuilder($"Request ID = {eventId}\r\n");
        sb.Append($"Path = {queryInfo.Path}\r\n");
        
        if(queryInfo.QueryParams != null)
            foreach(var item in queryInfo.QueryParams)
                sb.Append($"{item.Key} = {item.Value}\r\n");
        
        sb.Append($"{queryInfo}\r\n");
        
        if(queryInfo.Body != null)
            sb.Append($"{queryInfo.Body}");
        
        sb.Append($"{ex.Message}\r\n\r\n");
        sb.Append($"{ex.StackTrace}");
    
        return sb.ToString();
    }
}