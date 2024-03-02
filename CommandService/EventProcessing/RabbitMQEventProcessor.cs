using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.EventProcessing;

enum EventType
{
  PlatformPublished,
  Undetermined
}

public class RabbitMQEventProcessor : IEventProcessor
{
  private readonly IServiceScopeFactory _scopeFactory;
  private readonly IMapper _mapper;

  public RabbitMQEventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
  {
    _scopeFactory = scopeFactory;
    _mapper = mapper;
  }

  public void ProcessEvent(string message)
  {

    var eventType = DetermineEvent(message);
    switch (eventType)
    {
      case EventType.PlatformPublished:
        AddPlatform(message);
        Console.WriteLine("--> Platform Added");
        break;
      default:
        Console.WriteLine("--> Ignoring Undetermined Event");
        break;
    }
  }

  private void AddPlatform(string message)
  {
    using (var scope = _scopeFactory.CreateScope())
    {
      var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
      var data = JsonSerializer.Deserialize<PlatformPublishedDto>(message);
      try
      {
        var platform = _mapper.Map<Platform>(data);
        if (!repository.ExternalPlatformExists(platform.ExternalId))
        {
          repository.CreatePlatform(platform);
          repository.SaveChanges();
        }
        else
        {
          Console.WriteLine("--> Platform already exists");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not add platform to the database: {ex.Message}");
      }
    }
  }

  private EventType DetermineEvent(string message)
  {
    var data = JsonSerializer.Deserialize<GenericEventDto>(message);
    switch (data.Event)
    {
      case "Platform_Published":
        Console.WriteLine("--> Platform Published Event Detected");
        return EventType.PlatformPublished;
      default:
        Console.WriteLine("--> Undetermined Event Detected");
        return EventType.Undetermined;
    }
  }
}