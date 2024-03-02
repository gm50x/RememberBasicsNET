using CommandService.Models;
using CommandService.SyncDataServices.Grpc;

namespace CommandService.Data;

public static class PrepareDatabase
{
  public static void PreparePopulation(IApplicationBuilder builder)
  {
    using (var serviceScope = builder.ApplicationServices.CreateScope())
    {
      var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
      var platforms = grpcClient.ReturnAllPlatforms();
      SeedData(serviceScope.ServiceProvider.GetService<ICommandRepository>(), platforms);
    }
  }

  private static void SeedData(ICommandRepository repository, IEnumerable<Platform> platforms)
  {
    Console.WriteLine("--> Seeding existing platforms");
    foreach (var platform in platforms)
    {
      if (!repository.ExternalPlatformExists(platform.ExternalId))
      {
        repository.CreatePlatform(platform);
      }
    }
    repository.SaveChanges();
  }
}