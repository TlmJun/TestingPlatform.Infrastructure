using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Domain.Enums;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Db;
using TestingPlatform.Infrastructure.Exceptions;
using static System.Net.Mime.MediaTypeNames;
using TestingPlatform.Infrastructure.Exceptions;

namespace TestingPlatform.Infrastructure.Repositories
{
    public class AnswerRepository(AppDbContext appDbContext, IMapper mapper) : IAnswerRepository
    {
        public async Task<List<AnswerDto>> GetAllAsync()
        {
            var answers = await appDbContext.Answers
                .Include(s => s.Text)
                .ToListAsync();
            return mapper.Map<List<AnswerDto>>(answers);
        }

        public async Task<AnswerDto> GetByIdAsync(int id)
        {
            var answer = await appDbContext.Answers
            .AsNoTracking()
            .FirstOrDefaultAsync(answer => answer.Id == id);

            if (answer == null)
            {
                throw new EntityNotFoundException("Ответ не найден");
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
            var answer = await appDbContext.Answers.FirstOrDefaultAsync(answer => answer.Id == AnswerDto.Id);

            if (answer == null)
            {
                throw new EntityNotFoundException("Ответ не найден");
            }

            answer.Text = AnswerDto.Text!;

            await appDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var test = await appDbContext.Tests.FirstOrDefaultAsync(test => test.Id == id);

            if (test == null)
            {
                throw new EntityNotFoundException("Ответ не найден");
            }

            appDbContext.Tests.Remove(test);
            await appDbContext.SaveChangesAsync();
        }

    }
}