using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TreeTest.BL.Exceptions;
using TreeTest.BL.Tools;
using TreeTest.Data;
using TreeTest.Dtos;

namespace TreeTest.Controllers;

[ApiController]
[Route("")]
public class JournalController : ControllerBase
{
    private readonly ILogger<JournalController> _logger;
    private readonly TreeContext _db;
    private static JsonSerializerSettings jsnSettings = new JsonSerializerSettings()
    {
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
        Formatting = Formatting.Indented
    }; 

    public JournalController(ILogger<JournalController> logger, TreeContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpPost("/api.user.journal.getRange")]
    public async Task<ActionResult<IEnumerable<EventInfoShort>>> GetJournalEvents([FromQuery]EventRangeDto dto, [FromBody]EventFilter filter)
    {
        try
        {
            DateTime from = DateTime.Parse(filter.From).ToUniversalTime();
            DateTime to = DateTime.Parse(filter.To).ToUniversalTime();
            var ct = _db.Events.Where(x => (x.CreatedAt > from || x.CreatedAt < to) && x.Text.Contains(filter.Search) ).Count();
            EventInfoShort[] events = _db.Events.Where(x => (x.CreatedAt > from || x.CreatedAt < to) && x.Text.Contains(filter.Search) )
                                                    .Skip(dto.Skip)
                                                    .Take(dto.Take)
                                                    .Select(o => new EventInfoShort{
                                                        Id = o.Id,
                                                        EventId = o.EventId,
                                                        CreatedAt = o.CreatedAt
                                                    })
                                                    .ToArray();
            EventSelectRangeDto result = new EventSelectRangeDto()
            {
                Skip = dto.Skip,
                Count = ct,
                Items = events
            };

            return Ok(result);
        }
        catch(Exception e)
        {
            ExceptionInfo exInfo = await ProcessException(e);
            
            return StatusCode(500, JsonConvert.SerializeObject(exInfo, jsnSettings));
        }
        
    }

    [HttpPost("/api.user.journal.getSingle")]
    public async Task<ActionResult<JournalEvent>> GetSingleEventInfo(long eventId)
    {
        try
        {
            JournalEvent? e = _db.Events.Where(x => x.EventId.Equals(eventId)).FirstOrDefault();
            
            if(e != null)
                return Ok(e);
            else
                return StatusCode(500, $"Internal server error ID = {eventId}.");
        }
        catch(Exception e)
        {
            ExceptionInfo exInfo = await ProcessException(e);
            
            return StatusCode(500, JsonConvert.SerializeObject(exInfo, jsnSettings));
        }
    }

    private async Task<ExceptionInfo> ProcessException(Exception e)
    {
        RequestInfoExctractor exctractor = new RequestInfoExctractor();
        QueryInfo queryInfo = await exctractor.Get(Request); 
        long eventId = ExceptionWriter.Save(_db, queryInfo, e);

        return new ExceptionInfo($"Internal server error ID = {eventId}.", eventId.ToString(), e.GetType().Name);
    }
}