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
    public class AnswerRepository(AppDbContext appDbContext, IMapper mapper) : IAnswerRepository
    {
        public async Task<List<AnswerDto>> GetAllAsync()
        {
            var answers = await appDbContext.Answer
                .Include(s => s.Text)
                .ToListAsync();
            return mapper.Map<List<AnswerDto>>(answers);
        }

        public async Task<AnswerDto> GetByIdAsync(int id)
        {
            var answer = await appDbContext.Answer
            .AsNoTracking()
            .FirstOrDefaultAsync(answer => answer.Id == id);

            if (answer == null)
            {
                throw new Exception("Ответ не найден.");
            }

            return mapper.Map<AnswerDto>(answer);
        }


        public async Task<int> CreateAsync(AnswerDto AnswerDto)
        {
            var test = mapper.Map<AnswerDto>(AnswerDto);

            test.Text = AnswerDto.Text;

            await appDbContext.AddAsync(test);
            await appDbContext.SaveChangesAsync();

            return test.Id;
        }

        public async Task UpdateAsync(AnswerDto AnswerDto)
        {
            var answer = await appDbContext.Answer.FirstOrDefaultAsync(answer => answer.Id == AnswerDto.Id);

            if (answer == null)
            {
                throw new Exception("Ответ не найден.");
            }

            answer.Text = AnswerDto.Text!;

            await appDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var test = await appDbContext.Test.FirstOrDefaultAsync(test => test.Id == id);

            if (test == null)
            {
                throw new Exception("Тест не найден.");
            }

            appDbContext.Test.Remove(test);
            await appDbContext.SaveChangesAsync();
        }
    }
}