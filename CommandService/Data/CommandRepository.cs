using CommandService.Models;

namespace CommandService.Data;

public class CommandRepository : ICommandRepository
{
  private readonly AppDbContext _context;
  public CommandRepository(AppDbContext context)
  {
    _context = context;
  }
  public void CreateCommand(int platformId, Command command)
  {
    ArgumentNullException.ThrowIfNull(command);
    command.PlatformId = platformId;
    _context.Commands.Add(command);
  }

  public void CreatePlatform(Platform platform)
  {
    ArgumentNullException.ThrowIfNull(platform);
    _context.Platforms.Add(platform);
  }

  public IEnumerable<Platform> GetAllPlatforms()
  {
    return _context.Platforms.ToList();
  }

  public Command GetCommand(int platformId, int commandId)
  {
    return _context.Commands.SingleOrDefault(c => c.Id == commandId && c.PlatformId == platformId);
  }

  public IEnumerable<Command> GetCommandsForPlatform(int platformId)
  {
    return _context.Commands.Where(c => c.PlatformId == platformId);
  }

  public bool PlatformExists(int platformId)
  {
    return _context.Platforms.Any(p => p.Id == platformId);
  }

  public bool ExternalPlatformExists(int externalPlatformId)
  {
    return _context.Platforms.Any(p => p.ExternalId == externalPlatformId);
  }

  public bool SaveChanges()
  {
    return _context.SaveChanges() >= 0;
  }
}