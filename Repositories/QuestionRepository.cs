using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Db;
using TestingPlatform.Infrastructure.Exceptions;

namespace TestingPlatform.Infrastructure.Repositories;

public class QuestionRepository(AppDbContext appDbContext, IMapper mapper) : IQuestionRepository
{
    public async Task<IEnumerable<QuestionDto>> GetAllAsync()
    {
        var questions = await appDbContext.Question.AsNoTracking().ToListAsync();
        return mapper.Map<IEnumerable<QuestionDto>>(questions);
    }

    public async Task<QuestionDto> GetByIdAsync(int id)
    {
        var question = await appDbContext.Question
            .AsNoTracking()
            .FirstOrDefaultAsync(question => question.Id == id);

        if (question == null)
        {
            throw new EntityNotFoundException("Вопрос не найден.");
        }

        return mapper.Map<QuestionDto>(question);
    }

    public async Task<int> CreateAsync(QuestionDto questionDto)
    {
        var question = mapper.Map<Question>(questionDto);

        var questionId = await appDbContext.AddAsync(question);
        await appDbContext.SaveChangesAsync();

        return questionId.Entity.Id;
    }

    public async Task UpdateAsync(QuestionDto questionDto)
    {
        var question = await appDbContext.Question.FirstOrDefaultAsync(question => question.Id == questionDto.Id);

        if (question == null)
        {
            throw new EntityNotFoundException("Вопрос не найден.");
        }

        question.AnswerType = questionDto.AnswerType;
        question.Text = questionDto.Text;
        question.MaxScore = question.MaxScore;
        question.Description = questionDto.Description;
        question.IsScoring = questionDto.IsScoring;
        question.TestId = questionDto.TestId;

        await appDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var question = await appDbContext.Question.FirstOrDefaultAsync(question => question.Id == id);

        if (question == null)
        {
            throw new EntityNotFoundException("Вопрос не найден.");
        }

        appDbContext.Question.Remove(question);
        await appDbContext.SaveChangesAsync();
    }
}

