
using Microsoft.EntityFrameworkCore;
using PlatformService.Controllers;
using PlatformService.Data;
using PlatformService.SyncDataServices;
using PlatformService.SyncDataServices.Http;

namespace PlatformService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        if (builder.Environment.IsDevelopment())
        {
            Console.WriteLine("Using InMemory Database");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("DevInMemory"));
        }
        else
        {
            var connectionString = builder.Configuration.GetConnectionString("PlatformsDatabase")!
                .Replace("$SERVER", Environment.GetEnvironmentVariable("DB_SERVER"))
                .Replace("$USER", Environment.GetEnvironmentVariable("DB_USER"))
                .Replace("$PASS", Environment.GetEnvironmentVariable("DB_PASS"));
            Console.WriteLine("Using SQLServer Database");
            Console.WriteLine($"Using SQLServer Database: ${connectionString}");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        }


        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
        builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
        var commandServiceBaseURL = builder.Configuration["CommandService:BaseURL"];
        Console.WriteLine($"--> {builder.Environment.EnvironmentName}");
        Console.WriteLine($"--> CommandService {commandServiceBaseURL}");

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        PrepareDb.PrepareDatabase(app, app.Environment);
        app.Run();
    }
}
