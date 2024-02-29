using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TreeTest.Controllers;

[Tags("user.partner")]
[ApiController]
[Route("")]
public class PartnerController : ControllerBase
{
    [HttpPost("/api.user.partner.rememberMe")]
    public ActionResult RememberMe([Required][FromQuery]string code)
    {
        HttpContext.Session.SetString("code", code);

        return Ok();
    }
}