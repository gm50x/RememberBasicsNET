using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[ApiController]
[Route("api/c/platforms/{platformId}/commands")]
public class CommandsController : ControllerBase
{
  private readonly ICommandRepository _repository;
  private readonly IMapper _mapper;

  public CommandsController(ICommandRepository repository, IMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;

  }

  [HttpGet]
  public ActionResult<IEnumerable<CommandReadDto>> GetAllCommandsForPlatform(int platformId)
  {
    Console.WriteLine($"---> Getting Command For Platform {platformId}");
    if (!_repository.PlatformExists(platformId))
    {
      return NotFound();
    }
    var commands = _repository.GetCommandsForPlatform(platformId);
    return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
  }

}