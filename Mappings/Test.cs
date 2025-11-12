using AutoMapper;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Domain.Models;

namespace TestingPlatform.Infrastructure.Mappings;

public class TestProfile : Profile
{
    public TestProfile()
    {
        CreateMap<Test, TestDto>().ReverseMap();
    }
}