using Microsoft.AspNetCore.Mvc;
using TreeTest.Data;
using TreeTest.Dtos;

namespace TreeTest.Controllers;

[ApiController]
[Route("")]
public class JournalController : ControllerBase
{
    private readonly ILogger<JournalController> _logger;
    private readonly TreeContext _db;

    public JournalController(ILogger<JournalController> logger, TreeContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpPost("/api.user.journal.getRange")]
    public ActionResult<IEnumerable<EventInfoShort>> GetJournalEvents([FromQuery]EventRangeDto dto, [FromBody]EventFilter filter)
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
            return StatusCode(500, e.Message);
        }
        
    }

    [HttpPost("/api.user.journal.getSingle")]
    public ActionResult<JournalEvent> GetSingleEventInfo(long eventId)
    {
        JournalEvent? e = _db.Events.Where(x => x.EventId.Equals(eventId)).FirstOrDefault();
        
        if(e != null)
            return Ok(e);
        else
            return StatusCode(500, $"Internal server error ID = {eventId}.");
    }
}