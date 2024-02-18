
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
        builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("DevInMemory"));

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
        builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
        var commandServiceBaseURL = builder.Configuration.GetSection("CommandService").GetValue<string>("BaseURL");
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

        PrepareDb.PreparePopulation(app);

        app.Run();
    }
}
