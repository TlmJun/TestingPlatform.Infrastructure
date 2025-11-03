using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Db;

namespace TestingPlatform.Infrastructure.Repositories;

public class GroupRepository(AppDbContext appDbContext, IMapper mapper) : IGroupRepository
{
    public async Task<List<GroupDto>> GetAllAsync()
    {
        var groups = await appDbContext.Groups
            .Include(g => g.Project)
            .Include(g => g.Direction)
            .Include(g => g.Course)
            .Include(g => g.Students)
            .AsNoTracking()
            .ToListAsync();

        return mapper.Map<List<GroupDto>>(groups);
    }

    public async Task<GroupDto> GetByIdAsync(int id)
    {
        var group = await appDbContext.Groups
            .Include(g => g.Project)
            .Include(g => g.Course)
            .Include(g => g.Direction)
            .AsNoTracking()
            .FirstOrDefaultAsync(group => group.Id == id);

        if (group == null)
        {
            throw new Exception("Группа не найдена.");
        }

        return mapper.Map<GroupDto>(group);
    }


    public async Task<int> CreateAsync(GroupDto groupDto)
    {
        var group = mapper.Map<Group>(groupDto);

        var direction = await appDbContext.Directions.FirstOrDefaultAsync(d => d.Id == groupDto.Direction.Id);
        if (direction is null)
        {
            throw new Exception("Направление не найдено.");
        }
        group.Direction = direction;

        var course = await appDbContext.Courses.FirstOrDefaultAsync(c => c.Id == groupDto.Course.Id);
        if (course is null)
        {
            throw new Exception("Курс не найден.");
        }
        group.Course = course;

        var project = await appDbContext.Projects.FirstOrDefaultAsync(p => p.Id == groupDto.Project.Id);
        if (project is null)
        {
            throw new Exception("Проект не найден.");
        }
        group.Project = project;

        var groupId = await appDbContext.AddAsync(group);
        await appDbContext.SaveChangesAsync();

        return groupId.Entity.Id;
    }

    public async Task UpdateAsync(GroupDto groupDto)
    {
        var group = await appDbContext.Groups.FirstOrDefaultAsync(group => group.Id == groupDto.Id);

        if (group == null)
        {
            throw new Exception("Группа не найдена.");
        }

        group.Name = groupDto.Name;
        group.CourseId = groupDto.Course.Id;
        group.DirectionId = groupDto.Direction.Id;
        group.ProjectId = groupDto.Project.Id;

        await appDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var group = await appDbContext.Groups.FirstOrDefaultAsync(group => group.Id == id);

        if (group == null)
        {
            throw new Exception("Группа не найдена.");
        }

        appDbContext.Groups.Remove(group);
        await appDbContext.SaveChangesAsync();
    }
}



