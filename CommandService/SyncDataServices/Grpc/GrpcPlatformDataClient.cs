using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;

namespace CommandService.SyncDataServices.Grpc;

public class GrpcPlatformDataClient : IPlatformDataClient
{
  private readonly IConfiguration _configuration;
  private readonly IMapper _mapper;

  public GrpcPlatformDataClient(IConfiguration configuration, IMapper mapper)
  {
    _configuration = configuration;
    _mapper = mapper;
  }
  public IEnumerable<Platform> ReturnAllPlatforms()
  {
    Console.WriteLine($"--> Getting all platforms via GRPC: {_configuration["GrpcPlatform"]}");
    var httpHandler = new HttpClientHandler();
    httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"]!, new GrpcChannelOptions { HttpHandler = httpHandler });
    var client = new GrpcPlatform.GrpcPlatformClient(channel);
    var request = new GetAllRequest();
    try
    {
      var reply = client.GetAllPlatforms(request);
      return _mapper.Map<IEnumerable<Platform>>(reply.Platform);

    }
    catch (Exception ex)
    {
      Console.WriteLine($"Failed calling GRPC Server: {ex.Message}");
      return [];
    }
  }
}