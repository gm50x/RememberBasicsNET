
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[ApiController]
[Route("api/c/[controller]")]
public class PlatformsController : ControllerBase
{
  private readonly ICommandRepository _repository;
  private readonly IMapper _mapper;

  public PlatformsController(ICommandRepository repository, IMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  [HttpGet]
  public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
  {
    Console.WriteLine("---> Getting Platforms");
    var platforms = _repository.GetAllPlatforms();
    return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
  }

  [HttpPost]
  public ActionResult TestInboundConnection()
  {
    var traceParent = HttpContext.TraceIdentifier;
    Console.WriteLine($"-->> Inbound POST #[CommandService] - {traceParent}");
    return Ok("Inbound test of Inbound CommandService");
  }
}