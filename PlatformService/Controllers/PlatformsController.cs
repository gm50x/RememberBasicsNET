using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices;

namespace PlatformService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformsController : ControllerBase
{
  private readonly ILogger<PlatformsController> _logger;
  private readonly IPlatformRepository _platformRepository;
  private readonly IMapper _mapper;
  private readonly ICommandDataClient _commandDataClient;
  private readonly IMessageBusClient _messabeBus;

  public PlatformsController(ILogger<PlatformsController> logger, IPlatformRepository platformRepository, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBus)
  {
    _logger = logger;
    _platformRepository = platformRepository;
    _mapper = mapper;
    _commandDataClient = commandDataClient;
    _messabeBus = messageBus;
  }

  [HttpGet(Name = "GetPlatforms")]
  public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
  {
    _logger.LogInformation("GetPlatformsWasInvoked");
    var platforms = _platformRepository.GetAllPlatforms();
    var platformDtos = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
    return Ok(platformDtos);
  }

  [HttpGet("{id}", Name = "GetPlatformById")]
  public ActionResult<PlatformReadDto> GetPlatformById(int id)
  {
    _logger.LogInformation("GetPlatformByIdWasInvoked");
    var platform = _platformRepository.GetPlatformById(id);
    if (platform != null)
    {
      var platformDto = _mapper.Map<PlatformReadDto>(platform);
      return Ok(platformDto);
    }
    return NotFound();
  }

  [HttpPost(Name = "CreatePlatform")]
  public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformDto)
  {
    _logger.LogInformation("CreatePlatform");
    var platform = _mapper.Map<Platform>(platformDto);
    _platformRepository.CreatePlatform(platform);
    _platformRepository.SaveChanges();
    var platformOutput = _mapper.Map<PlatformReadDto>(platform);
    try
    {
      await _commandDataClient.SendPlatformToCommand(platformOutput);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"--> Failed sending synchronously: {ex.Message}");
    }

    try
    {
      var publishedDto = _mapper.Map<PlatformPublishedDto>(platformOutput);
      publishedDto.Event = "Platform_Published";
      _messabeBus.PublishNewPlatform(publishedDto);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"--> Failed sending asynchronous message: {ex.Message}");
    }



    return CreatedAtRoute(nameof(GetPlatformById), new { platformOutput.Id }, platformOutput);
  }

}