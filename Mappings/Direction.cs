using AutoMapper;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Domain.Models;

namespace TestingPlatform.Infrastructure.Mappings;

public class DirectionProfile : Profile
{
    public DirectionProfile()
    {
        CreateMap<Domain.Models.Direction, DirectionDto>().ReverseMap();
    }
}
