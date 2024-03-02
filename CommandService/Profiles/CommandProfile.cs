using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.Profiles;

public class CommandProfile : Profile
{
  public CommandProfile()
  {
    CreateMap<Platform, PlatformReadDto>();
    CreateMap<CommandCreateDto, Command>();
    CreateMap<Command, CommandReadDto>();
    CreateMap<PlatformPublishedDto, Platform>()
      .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
    CreateMap<GrpcPlatformModel, Platform>()
      .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.PlatformId))
      .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
      .ForMember(dest => dest.Commands, opt => opt.Ignore());
  }
}