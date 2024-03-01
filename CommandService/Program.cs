
using CommandService.Data;
using Microsoft.EntityFrameworkCore;

namespace CommandService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMemoryDb"));
        builder.Services.AddControllers();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddScoped<ICommandRepository, CommandRepository>();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

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

        app.Run();
    }
}
