
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[ApiController]
[Route("api/c/[controller]")]
public class PlatformsController : ControllerBase
{
  public PlatformsController()
  {
  }

  [HttpPost]
  public ActionResult TestInboundConnection()
  {
    var traceId = HttpContext.TraceIdentifier;
    Console.WriteLine($"-->> Inbound POST #[CommandService] - {traceId}");
    return Ok("Inbound test of Inbound CommandService");
  }
}