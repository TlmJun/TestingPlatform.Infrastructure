using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Db;
using TestingPlatform.Infrastructure.Exceptions;

namespace TestingPlatform.Infrastructure.Repositories;

public class AttemptRepository(AppDbContext appDbContext, IMapper mapper, ITestRepository testRepository) : IAttemptRepository
{
    public async Task<int> CreateAsync(AttemptDto attemptDto)
    {
        var test = await testRepository.GetByIdAsync(attemptDto.TestId);

        if (test is null)
            throw new EntityNotFoundException("Тест не найден.");

        var student = await appDbContext.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == attemptDto.StudentId);

        // ошибка, если студент не найден
        if (student == null)
            throw new EntityNotFoundException("Студент не найден.");

        // ошибка, если тест не опубликован
        if (!test.IsPublic)
            throw new InvalidOperationException("Тест не доступен");

        // среди доступных ученику тестов нет, того, который он собирается проходить
        var availableTests = await testRepository.GetAllForStudent(attemptDto.StudentId);

        if (availableTests.All(t => t.Id != attemptDto.TestId))
            throw new InvalidOperationException("Доступ запрещен");

        var attempt = mapper.Map<Attempt>(attemptDto);

        // если тест повторяемый и попытки неограничены, то сразу даем доступ к попытке
        if (test is { IsRepeatable: true, MaxAttempts: null })
            return await CreateAsync(attempt);

        // предыдущие попытки студента
        var lastAttempts = await appDbContext.Attempts
            .Where(a => a.StudentId == attemptDto.StudentId && a.TestId == attemptDto.TestId)
            .ToListAsync();

        // обработку незавершенных попыток
        var inProgress = lastAttempts.FirstOrDefault(a => a.SubmittedAt == null);
        if (inProgress != null)
        {
            if (test.DurationMinutes.HasValue)
            {
                var expiresAt = inProgress.StartedAt.AddMinutes(test.DurationMinutes.Value);
                if (DateTimeOffset.UtcNow < expiresAt)
                {
                    throw new InvalidOperationException("Есть незавершённая попытка, время выполнения ещё не истекло.");
                }
                // Если уже истёк -- разрешаем начать новую попытку (можно, если нужно, пометить старую попытку как "автозавершена")
            }
            else
            {
                // Если длительность не задана и попытка незавершена -- считаем, что новую попытку начинать нельзя
                throw new InvalidOperationException("Есть незавершённая попытка. Тест не имеет ограничений по времени, поэтому новую попытку начать нельзя.");
            }
        }

        // ошибка, если пытаются пройти тест повторно, который уже пройден
        if (!test.IsRepeatable && lastAttempts.Count > 0)
            throw new InvalidOperationException("Тест нельзя пройти более одного раза");

        if (test.IsRepeatable && lastAttempts.Count >= test.MaxAttempts)
            throw new InvalidOperationException("Исчерпано количество прохождений теста");

        return await CreateAsync(attempt);
    }

    private async Task<int> CreateAsync(Attempt attempt)
    {
        attempt.StartedAt = DateTime.Now;
        attempt.Score = 0;
        var attemptId = await appDbContext.AddAsync(attempt);
        await appDbContext.SaveChangesAsync();

        return attemptId.Entity.Id;
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
}

