
using System.Text;
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
  private readonly IConfiguration _configuration;
  private readonly IEventProcessor _eventProcessor;
  private IConnection _connection;
  private IModel _channel;
  private string _queueName;

  public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
  {
    _configuration = configuration;
    _eventProcessor = eventProcessor;
    InitializeRabbitMQ();
  }

  protected override Task ExecuteAsync(CancellationToken stoppingToken)
  {
    stoppingToken.ThrowIfCancellationRequested();
    var consumer = new EventingBasicConsumer(_channel);
    consumer.Received += (ModuleHandle, ea) =>
    {
      Console.WriteLine("--> Received Messaage");
      var message = Encoding.UTF8.GetString(ea.Body.ToArray());
      _eventProcessor.ProcessEvent(message);
    };

    _channel.BasicConsume(_queueName, true, consumer);
    return Task.CompletedTask;
  }

  private void InitializeRabbitMQ()
  {
    var factory = new ConnectionFactory { Uri = new Uri(_configuration["AmqpURL"]!) };
    _connection = factory.CreateConnection();
    _channel = _connection.CreateModel();

    _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
    _queueName = _channel.QueueDeclare().QueueName;
    _channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");

    Console.WriteLine($"--> Consuming queue {_queueName}");
    _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

  }

  private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs args)
  {
    Console.WriteLine("--> Connection has been shutdown");
  }

  public override void Dispose()
  {
    Console.WriteLine($"--> Disposing {nameof(MessageBusSubscriber)}");
    if (_channel.IsOpen)
    {
      _channel.Close();
      _connection.Close();
    }
    base.Dispose();
  }
}