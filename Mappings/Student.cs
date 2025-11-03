using AutoMapper;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Domain.Models;

namespace TestingPlatform.Infrastructure.Mappings;

public class StudentProfile : Profile
{
    public StudentProfile()
    {
        CreateMap<Student, StudentDto>()
            .ForMember(d => d.User, m => m.MapFrom(s => s.User));

        CreateMap<StudentDto, Student>()
            .ForMember(d => d.User, m => m.Ignore());
    }
}

