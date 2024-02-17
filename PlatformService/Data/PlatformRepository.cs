using PlatformService.Models;

namespace PlatformService.Data;

public class PlatformRepository : IPlatformRepository
{
  private readonly AppDbContext _context;

  public PlatformRepository(AppDbContext context)
  {
    _context = context;
  }

  bool IPlatformRepository.SaveChanges()
  {
    return _context.SaveChanges() >= 0;
  }

  IEnumerable<Platform> IPlatformRepository.GetAllPlatforms()
  {
    return _context.Platforms.ToList();
  }

  Platform IPlatformRepository.GetPlatformById(int id)
  {
    return _context.Platforms.FirstOrDefault(p => p.Id == id);
  }

  void IPlatformRepository.CreatePlatform(Platform platform)
  {
    ArgumentNullException.ThrowIfNull(platform);
    _context.Platforms.Add(platform);
  }
}