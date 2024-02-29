using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TreeTest.BL.Exceptions;
using TreeTest.BL.Tools;
using TreeTest.Data;
using TreeTest.Dtos;

namespace TreeTest.Controllers;

/// <summary>
/// Represents journal API
/// </summary>
[Tags("user.journal")]
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

    /// <remarks>
    /// Provides the pagination API. Skip means the number of items should be skipped by server. Take means the maximum number items should be returned by server. All fields of the filter are optional.
    /// </remarks>
    [HttpPost("/api.user.journal.getRange")]
    public async Task<ActionResult<IEnumerable<EventInfoShort>>> GetJournalEvents([FromQuery]EventRangeDto dto, [FromBody]EventFilter filter)
    {
        try
        {
            bool isFromParsed;
            bool isToParsed;
            DateTime parsedFrom;
            DateTime parsedTo;
            isFromParsed = DateTime.TryParse(filter.From, out parsedFrom);
            isToParsed = DateTime.TryParse(filter.To, out parsedTo);
            
            var evts = _db.Events.Where(x => x.Text.Contains(filter.Search) );
            
            if(isFromParsed)
                evts = evts.Where(x => x.CreatedAt >= parsedFrom.ToUniversalTime());

            if(isToParsed)
                evts = evts.Where(x => x.CreatedAt <= parsedTo.ToUniversalTime());

            int ct = evts.Count();

            EventInfoShort[] events = evts.Skip(dto.Skip)
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

    /// <remarks>Returns the information about an particular event by ID.</remarks>
    [HttpPost("/api.user.journal.getSingle")]
    public async Task<ActionResult<JournalEvent>> GetSingleEventInfo([Required]long eventId)
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