using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class PrepareDb
{
  public static void PrepareDatabase(IApplicationBuilder app, IWebHostEnvironment environment)
  {
    using (var serviceScope = app.ApplicationServices.CreateScope())
    {
      var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
      ArgumentNullException.ThrowIfNull(context);
      if (environment.IsProduction())
      {
        RunMigrations(context);
      }
      SeedData(context);
    }
  }

  private static void RunMigrations(AppDbContext context)
  {
    try
    {
      Console.WriteLine($"--> Applying Migrations");
      context.Database.Migrate();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"--> Could not run migrations: {ex.Message}");
    }
  }

  private static void SeedData(AppDbContext context)
  {
    if (!context.Platforms.Any())
    {
      Thread.Sleep(1000);
      Console.WriteLine("---> Seeding data...");
      context.Platforms.AddRange(
        new Platform { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
        new Platform { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
        new Platform { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
      );
      context.SaveChanges();
    }
    else
    {
      Console.WriteLine("---> Data has already been seeded");
    }

  }
}
