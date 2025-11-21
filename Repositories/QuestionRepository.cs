using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Db;
using TestingPlatform.Infrastructure.Exceptions;

namespace TestingPlatform.Infrastructure.Repositories;

public class QuestionRepository(AppDbContext appDbContext, IMapper mapper, ILogger<QuestionRepository> logger) : IQuestionRepository
{
    public async Task<List<QuestionDto>> GetAllAsync()
    {
        logger.LogInformation("Получен запрос на получение всех вопросов");
        var questions = await appDbContext.Question.AsNoTracking().ToListAsync();

        logger.LogInformation("Из базы получено {Count} вопросов", questions.Count);
        return mapper.Map<List<QuestionDto>>(questions);
    }

    public async Task<QuestionDto> GetByIdAsync(int id)
    {
        logger.LogInformation("Запрос на получение вопроса Id={QuestionId}", id);

        var question = await appDbContext.Question
            .AsNoTracking()
            .FirstOrDefaultAsync(question => question.Id == id);

        if (question == null)
        {
            throw new EntityNotFoundException("Вопрос не найден.");
        }

        logger.LogInformation("Вопрос Id={QuestionId} успешно найден", id);
        return mapper.Map<QuestionDto>(question);
    }

    public async Task<int> CreateAsync(QuestionDto questionDto)
    {
        logger.LogInformation("Создание нового вопроса для теста TestId={TestId}", questionDto.TestId);
        var question = mapper.Map<Question>(questionDto);

        var questionId = await appDbContext.AddAsync(question);
        await appDbContext.SaveChangesAsync();

        logger.LogInformation("Вопрос успешно создан с Id={QuestionId}", questionId.Entity.Id);
        return questionId.Entity.Id;
    }

    public async Task UpdateAsync(QuestionDto questionDto)
    {
        logger.LogInformation("Обновление вопроса Id={QuestionId}", questionDto.Id);

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

        logger.LogInformation("Вопрос Id={QuestionId} успешно обновлён", questionDto.Id);
    }

    public async Task DeleteAsync(int id)
    {
        logger.LogInformation("Удаление вопроса Id={QuestionId}", id);

        var question = await appDbContext.Question.FirstOrDefaultAsync(question => question.Id == id);

        if (question == null)
        {
            throw new EntityNotFoundException("Вопрос не найден.");
        }

        appDbContext.Question.Remove(question);
        await appDbContext.SaveChangesAsync();

        logger.LogInformation("Вопрос Id={QuestionId} успешно удалён", id);
    }
}

