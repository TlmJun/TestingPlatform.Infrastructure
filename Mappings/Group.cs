using AutoMapper;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Domain.Models;

namespace TestingPlatform.Infrastructure.Mappings;

public class GroupProfile : Profile
{
    public GroupProfile()
    {
        CreateMap<Group, GroupDto>();
        CreateMap<GroupDto, Group>()
            .ForMember(d => d.Course, o => o.Ignore())
            .ForMember(d => d.Direction, o => o.Ignore())
            .ForMember(d => d.Project, o => o.Ignore());
    }
}
