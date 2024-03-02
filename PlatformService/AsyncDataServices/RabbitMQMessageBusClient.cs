using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class RabbitMQMessageBusClient : IMessageBusClient, IDisposable
{
  private readonly IConfiguration _configuration;
  private readonly IConnection _connection;
  private readonly IModel _channel;

  public RabbitMQMessageBusClient(IConfiguration configuration)
  {
    Console.WriteLine("---> Creating RabbitMQ Message Bus");
    _configuration = configuration;
    var factory = new ConnectionFactory
    {
      Uri = new Uri(_configuration["AmqpURL"]!)
    };

    try
    {
      _connection = factory.CreateConnection();
      _channel = _connection.CreateModel();
      _channel.ExchangeDeclare("trigger", ExchangeType.Fanout);
      _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
      Console.WriteLine("---> Connected to the message bus");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"--> Could not connect to the message bus: {ex.Message}");
      Console.WriteLine($"--> {_configuration["AmqpURL"]}");
    }
  }

  private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs args)
  {
    Console.WriteLine("---> Connection has been shutdown");
  }

  public void PublishNewPlatform(PlatformPublishedDto platform)
  {
    var message = JsonSerializer.Serialize(platform);
    if (_connection.IsOpen)
    {
      Console.WriteLine("---> RabbitMQ Connection is open, sending message...");
      SendMessage(message);
    }
    else
    {
      Console.WriteLine("--> RabbitMQ Connection is closed, not sending message...");
    }
  }

  private void SendMessage(string message)
  {
    var body = Encoding.UTF8.GetBytes(message);
    _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);
  }

  public void Dispose()
  {
    Console.WriteLine("--> Message Bus Disposed");
    if (_channel.IsOpen)
    {
      _channel.Close();
      _connection.Close();
    }
  }
}