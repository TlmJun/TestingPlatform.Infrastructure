using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Domain.Enums;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Db;
using TestingPlatform.Infrastructure.Exceptions;

namespace TestingPlatform.Infrastructure.Repositories;
public class StudentAnswerRepository(AppDbContext appDbContext, IMapper mapper) : IStudentAnswerRepository
{
    public async Task CreateAsync(UserAttemptAnswerDto userAttemptAnswerDto)
    {
        var attempt = await appDbContext.Attempts
            .Include(a => a.UserAttemptAnswers)
            .FirstOrDefaultAsync(a => a.Id == userAttemptAnswerDto.AttemptId);

        if (attempt == null)
            throw new EntityNotFoundException("Попытка не найдена.");

        if (attempt.SubmittedAt != null)
            throw new InvalidOperationException("Нельзя добавлять ответ в уже сданную попытку.");

        var question = await appDbContext.Question
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == userAttemptAnswerDto.QuestionId);

        if (question == null)
            throw new EntityNotFoundException("Вопрос не найден.");

        var userAttemptAnswer = new UserAttemptAnswer
        {
            AttemptId = attempt.Id,
            QuestionId = question.Id,
            UserSelectedOptions = new List<UserSelectedOption>(),
            UserTextAnswers = null,
            IsCorrect = false,
            ScoreAwarded = 0
        };

        switch (question.AnswerType)
        {
            case AnswerType.Single:
                {
                    var selected = userAttemptAnswerDto.UserSelectedOptions?.FirstOrDefault();

                    if (selected == 0 || selected is null)
                        throw new InvalidOperationException("Ожидается выбранный вариант ответа.");

                    var selectedAnswerEntity = question.Answers.FirstOrDefault(a => a.Id == selected);
                    if (selectedAnswerEntity == null)
                        throw new EntityNotFoundException("Выбранный вариант ответа не найден.");

                    userAttemptAnswer.IsCorrect = selectedAnswerEntity.IsCorrect;

                    if (question.IsScoring)
                    {
                        var max = question.MaxScore ?? 1;
                        userAttemptAnswer.ScoreAwarded = selectedAnswerEntity.IsCorrect ? max : 0;
                    }
                    else
                    {
                        userAttemptAnswer.ScoreAwarded = 0;
                    }

                    userAttemptAnswer.UserSelectedOptions.Add(new UserSelectedOption
                    {
                        AnswerId = (int)selected
                    });

                    break;
                }

            case AnswerType.Multiple:
                {
                    var selectedIds = userAttemptAnswerDto.UserSelectedOptions ?? new List<int>();
                    if (selectedIds.Count == 0)
                        throw new InvalidOperationException("Ожидается как минимум один выбранный вариант для множественного выбора.");

                    // набор правильных вариантов
                    var correctAnswerIds = question.Answers
                        .Where(a => a.IsCorrect)
                        .Select(a => a.Id)
                        .ToHashSet();

                    var allAnswerIds = question.Answers.Select(a => a.Id).ToHashSet();
                    if (selectedIds.Any(id => !allAnswerIds.Contains(id)))
                        throw new EntityNotFoundException("Один или несколько выбранных вариантов не существуют в вопросе.");

                    var selectedSet = selectedIds.ToHashSet();
                    var isExactMatch = selectedSet.SetEquals(correctAnswerIds);

                    userAttemptAnswer.IsCorrect = isExactMatch;

                    if (question.IsScoring)
                    {
                        var max = question.MaxScore ?? 1;

                        userAttemptAnswer.ScoreAwarded = isExactMatch ? max : 0;
                    }
                    else
                    {
                        userAttemptAnswer.ScoreAwarded = 0;
                    }

                    foreach (var aid in selectedIds)
                    {
                        userAttemptAnswer.UserSelectedOptions.Add(new UserSelectedOption
                        {
                            AnswerId = aid
                        });
                    }

                    break;
                }

            case AnswerType.Text:
                {
                    var text = userAttemptAnswerDto.UserTextAnswers?.Trim();

                    userAttemptAnswer.UserTextAnswers = new UserTextAnswer
                    {
                        TextAnswer = text
                    };

                    if (!string.IsNullOrEmpty(text))
                    {
                        userAttemptAnswer.IsCorrect = true;
                        userAttemptAnswer.ScoreAwarded = question.MaxScore ?? 0;
                    }
                    else
                    {
                        userAttemptAnswer.IsCorrect = false;
                        userAttemptAnswer.ScoreAwarded = 0;
                    }

                    break;
                }

            default:
                throw new NotSupportedException($"Тип ответа {question.AnswerType} не поддерживается.");
        }

        await appDbContext.AddAsync(userAttemptAnswer);
        await appDbContext.SaveChangesAsync();
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