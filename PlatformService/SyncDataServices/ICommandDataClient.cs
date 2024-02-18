using PlatformService.Dtos;

namespace PlatformService.SyncDataServices;

public interface ICommandDataClient
{
  public Task SendPlatformToCommand(PlatformReadDto platformData);
}