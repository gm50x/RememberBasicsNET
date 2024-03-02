using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
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
    Console.WriteLine($"---> Getting All Commands For Platform {platformId}");
    if (!_repository.PlatformExists(platformId))
    {
      return NotFound();
    }
    var commands = _repository.GetCommandsForPlatform(platformId);
    return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
  }

  [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
  public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
  {
    Console.WriteLine($"---> Getting Command {commandId} For Platform {platformId}");
    if (!_repository.PlatformExists(platformId))
    {
      return NotFound();
    }
    var command = _repository.GetCommand(platformId, commandId);
    if (command == null)
    {
      return NotFound();
    }
    return Ok(_mapper.Map<CommandReadDto>(command));
  }

  [HttpPost(Name = "CreateCommandForPlatform")]
  public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
  {
    Console.WriteLine($"---> Creating Command For Platform {platformId}");
    if (!_repository.PlatformExists(platformId))
    {
      return NotFound();
    }
    var command = _mapper.Map<Command>(commandDto);
    _repository.CreateCommand(platformId, command);
    _repository.SaveChanges();
    if (command == null)
    {
      return NotFound();
    }
    var commandOutput = _mapper.Map<CommandReadDto>(command);
    return CreatedAtRoute(
      nameof(GetCommandForPlatform),
      new { platformId, commandId = commandOutput.Id },
      commandOutput
    );

  }

}