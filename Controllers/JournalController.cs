using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public ActionResult<IEnumerable<RangeDto>> GetJournalEvents(RangeDto dto)
    {

        return new List<RangeDto>(){dto}.ToArray();
    }
}