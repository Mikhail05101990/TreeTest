using Microsoft.AspNetCore.Mvc;

namespace TreeTest.Controllers;

[ApiController]
[Route("")]
public class PartnerController : ControllerBase
{
    [HttpPost("/api.user.partner.rememberMe")]
    public ActionResult RememberMe([FromQuery]string code)
    {
        HttpContext.Session.SetString("code", code);

        return Ok();
    }
}