using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
  private readonly HttpClient _httpClient;
  private readonly IConfiguration _configuration;

  public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
  {
    _httpClient = httpClient;
    _configuration = configuration;
  }

  public async Task SendPlatformToCommand(PlatformReadDto platformData)
  {
    var config = _configuration.GetSection("CommandService");
    var baseURL = config.GetValue<string>("BaseURL");
    var createPlatformsURI = config.GetValue<string>("CreatePlatformsURI");
    var url = $"{baseURL}{createPlatformsURI}";
    var httpContent = new StringContent(
      JsonSerializer.Serialize(platformData),
      Encoding.UTF8,
      "application/json");
    var response = await _httpClient.PostAsync(url, httpContent);
    if (response.IsSuccessStatusCode)
    {
      Console.WriteLine("--> Sync POST to CommandService SUCCEEDED!");
      Console.WriteLine(response.Content);
    }
    else
    {
      Console.WriteLine("--> Sync POST to CommandService FAILED!");
    }
  }
}