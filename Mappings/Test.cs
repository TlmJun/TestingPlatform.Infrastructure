using AutoMapper;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Domain.Models;

namespace TestingPlatform.Infrastructure.Mappings;

public class TestProfile : Profile
{
    public TestProfile()
    {
        CreateMap<Test, TestDto>();
        CreateMap<TestDto, Test>()
            .ForMember(d => d.Questions, o => o.Ignore())
            .ForMember(d => d.Students, o => o.Ignore())
            .ForMember(d => d.Projects, o => o.Ignore())
            .ForMember(d => d.Courses, o => o.Ignore())
            .ForMember(d => d.Groups, o => o.Ignore())
            .ForMember(d => d.Directions, o => o.Ignore());
    }
}

