using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Db;
using TestingPlatform.Infrastructure.Exceptions;

namespace TestingPlatform.Infrastructure.Repositories;

public class StudentRepository(AppDbContext appDbContext, IMapper mapper) : IStudentRepository
{
    public async Task<IEnumerable<StudentDto>> GetAllAsync()
    {
        var students = await appDbContext.Students
            .Include(s => s.User)
            .ToListAsync();
        return mapper.Map<IEnumerable<StudentDto>>(students);
    }
    public async Task<StudentDto?> GetStudentByUserId(int userId)
    {
        var student = await appDbContext.Students
            .Include(s => s.User)
            .Include(s => s.Tests)
            .AsNoTracking()
            .FirstOrDefaultAsync(student => student.UserId == userId);

        if (student == null)
        {
            return null;
        }

        return mapper.Map<StudentDto>(student);
    }


    public async Task<StudentDto> GetByIdAsync(int id)
    {
        var student = await appDbContext.Students
            .Include(s => s.User)
            .Include(s => s.Tests)
            .AsNoTracking()
            .FirstOrDefaultAsync(student => student.Id == id);

        if (student == null)
        {
            throw new EntityNotFoundException("Студент не найден");
        }

        return mapper.Map<StudentDto>(student);
    }

    public async Task<int> CreateAsync(StudentDto studentDto)
    {
        var student = mapper.Map<Student>(studentDto);

        var studentId = await appDbContext.AddAsync(student);
        await appDbContext.SaveChangesAsync();

        return studentId.Entity.Id;
    }

    public async Task UpdateAsync(StudentDto studentDto)
    {
        var student = await appDbContext.Students.FirstOrDefaultAsync(student => student.Id == studentDto.Id);

        if (student == null)
        {
            throw new EntityNotFoundException("Студент не найден");
        }

        student.Phone = studentDto.Phone;
        student.VkProfileLink = studentDto.VkProfileLink;

        await appDbContext.SaveChangesAsync();
    }

    public async Task UpdateAvatarAsync(StudentDto studentDto)
    {
        var student = await appDbContext.Students.FirstOrDefaultAsync(student => student.Id == studentDto.Id);

        if (student == null)
        {
            throw new EntityNotFoundException("Студент не найден.");
        }

        student.AvatarPath = studentDto.AvatarPath;
        await appDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var student = await appDbContext.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
        {
            throw new EntityNotFoundException("Студент не найден");
        }

        appDbContext.Users.Remove(student.User);

        await appDbContext.SaveChangesAsync();
    }
}

