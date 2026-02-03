using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Domain.Enums;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Db;
using TestingPlatform.Infrastructure.Exceptions;
using static System.Net.Mime.MediaTypeNames;

namespace TestingPlatform.Infrastructure.Repositories;

public class AnswerRepository(AppDbContext appDbContext, IMapper mapper) : IAnswerRepository
{
    public async Task<int> CreateAsync(AnswerDto answerDto)
    {
        var answer = mapper.Map<Answer>(answerDto);

        var question = await appDbContext.Question
            .Include(question => question.Answers)
            .FirstOrDefaultAsync(question => question.Id == answerDto.QuestionId);

        if (question is null)
            throw new EntityNotFoundException("Вопрос не найден.");

        if (question.AnswerType == AnswerType.Text)
            throw new ArgumentException("К текстовому вопросу нельзя добавить ответ");

        if (question.AnswerType == AnswerType.Single && answer.IsCorrect && question.Answers.Any(a => a.IsCorrect))
            throw new ArgumentException("В данном типе теста можно выбрать только один правильный ответ");

        var answerId = await appDbContext.AddAsync(answer);
        await appDbContext.SaveChangesAsync();

        return answerId.Entity.Id;
    }

    public async Task UpdateAsync(AttemptDto attemptDto)
    {
        var attempt = await appDbContext.Attempts
            .Include(attempt => attempt.UserAttemptAnswers)
            .FirstOrDefaultAsync(a => a.Id == attemptDto.Id);

        if (attempt is null)
            throw new EntityNotFoundException("Попытка не найдена");

        if (attempt.SubmittedAt != null)
            throw new InvalidOperationException("Нельзя завершить уже сданную попытку.");

        attempt.SubmittedAt = DateTime.Now;

        var score = attempt.UserAttemptAnswers.Sum(ua => ua.ScoreAwarded);
        attempt.Score = score;

        var test = await appDbContext.Tests.AsNoTracking().FirstOrDefaultAsync(test => test.Id == attempt.TestId);

        var testResult = await appDbContext.TestResults
            .Include(tr => tr.Attempt)
            .FirstOrDefaultAsync(tr => tr.TestId == attempt.TestId);

        if (testResult == null)
        {
            var newtestResult = new TestResult
            {
                AttemptId = attempt.Id,
                StudentId = attempt.StudentId,
                TestId = attempt.TestId,
                Passed = test!.PassingScore == null || test.PassingScore <= attempt.Score, // если не указан проходной балл, то true, иначе сравниваем с наилучшей попыткой
            };

            await appDbContext.TestResults.AddAsync(newtestResult);
        }
        else
        {
            if (testResult.Attempt.Score < attempt.Score)
            {
                testResult.AttemptId = attempt.Id;
                testResult.Passed = test!.PassingScore == null || test.PassingScore <= attempt.Score;
            }
        }

        await appDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int answerId)
    {
        var answer = await appDbContext.Answers.FirstOrDefaultAsync(answer => answer.Id == answerId);

        if (answer == null)
        {
            throw new EntityNotFoundException("Ответ не найден.");
        }

        appDbContext.Answers.Remove(answer);
        await appDbContext.SaveChangesAsync();
    }

}
