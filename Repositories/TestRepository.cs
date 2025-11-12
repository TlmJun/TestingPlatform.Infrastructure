using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Domain.Enums;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Db;
using static System.Net.Mime.MediaTypeNames;

namespace TestingPlatform.Infrastructure.Repositories
{
    public class TestRepository(AppDbContext appDbContext, IMapper mapper) : ITestRepository
    {
        public async Task<IEnumerable<TestDto>> GetAllAsync()
        {
            var tests = await appDbContext.Test
                .Include(t => t.Title)
                .ToListAsync();
            return mapper.Map<List<TestDto>>(tests);
        }

        public async Task<TestDto> GetByIdAsync(int id)
        {
            var test = await appDbContext.Test
            .AsNoTracking()
            .FirstOrDefaultAsync(test => test.Id == id);

            if (test == null)
            {
                throw new Exception("Тест не найден.");
            }

            return mapper.Map<TestDto>(test);
        }


        public async Task<int> CreateAsync(TestDto TestDto)
        {
            var test = mapper.Map<TestDto>(TestDto);

            test.Title = TestDto.Title;
            test.Description = TestDto.Description;

            await appDbContext.AddAsync(test);
            await appDbContext.SaveChangesAsync();

            return test.Id;
        }

        public async Task UpdateAsync(TestDto TestDto)
        {
            var test = await appDbContext.Test.FirstOrDefaultAsync(test => test.Id == TestDto.Id);

            if (test == null)
            {
                throw new Exception("Тест не найден.");
            }

            if (appDbContext.Test.Any(u => u.Title == TestDto.Title))
                throw new ArgumentException($"Тест с логином {TestDto.Title} уже существует.");

            test.Title = TestDto.Title;
            test.Description = TestDto.Description;

            await appDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var test = await appDbContext.Test.FirstOrDefaultAsync(test =>test.Id == id);

            if (test == null)
            {
                throw new Exception("Тест не найден.");
            }

            appDbContext.Test.Remove(test);
            await appDbContext.SaveChangesAsync();
        }
    }
}
