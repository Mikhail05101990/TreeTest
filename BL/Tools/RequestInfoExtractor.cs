using TreeTest.Dtos;

namespace TreeTest.BL.Tools;

public class RequestInfoExctractor
{
    public async Task<QueryInfo> Get(HttpRequest request)
    {
        var stream = new StreamReader(request.Body);
        string body = await stream.ReadToEndAsync();
        
        return new QueryInfo()
        {
            Path = string.Format("{0}",request.Path.Value),
            QueryParams = request.Query,
            Body = body
        };
    }
}